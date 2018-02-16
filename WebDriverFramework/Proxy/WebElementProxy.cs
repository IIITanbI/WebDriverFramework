namespace WebDriverFramework.Proxy
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Internal;
    using OpenQA.Selenium.Remote;
    using OpenQA.Selenium.Support.PageObjects;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Messaging;

    public interface IProxiable
    {
        bool IsCached { get; }
    }

    public class WebElementProxy : DriverObjectProxy, IRemotingTypeInfo, IProxiable, IWrapsElement
    {
        private static List<Type> InterfacesToBeProxied => new List<Type>
        {
            typeof(ILocatable),
            typeof(IWebElement),
            typeof(IWrapsElement),
            typeof(IProxiable)
        };

        private IWebElement cachedElement;

        public WebElementProxy(IWebElement element) : this(null, null, true)
        {
            this.cachedElement = element;
        }
        public WebElementProxy(By by, WebElement source, bool shouldCached = false)
            : this(typeof(IWebElement), null, new[] { by }, shouldCached)
        {
            this.Source = source;
        }
        public WebElementProxy(Type typeToBeProxied, IElementLocator locator, IEnumerable<By> bys, bool shouldCached)
            : base(typeToBeProxied, locator, bys, shouldCached)
        {
        }
       
        public WebElement Source;

        public bool IsCached => (cachedElement as IProxiable)?.IsCached ?? cachedElement is RemoteWebElement;

        public IWebElement WrappedElement
        {
            get
            {
                if (this.cachedElement != null)
                {
                    return this.cachedElement;
                }

                IWebElement element;
                if (this.Source != null)
                {
                    var context = this.Source.Parent?.Element ?? (ISearchContext)this.Source.WrappedDriver;
                    element = new DefaultElementLocator(context).LocateElement(this.Bys);
                }
                else
                {
                    element = this.Locator.LocateElement(this.Bys);
                }

                if (this.ShouldCached)
                {
                    this.cachedElement = element;
                }

                return element;
            }
        }

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage methodCallMessage = msg as IMethodCallMessage;

            var declaringType = (methodCallMessage.MethodBase as MethodInfo).DeclaringType;
            if (typeof(IProxiable) == declaringType)
            {
                return InvokeMethod(this, methodCallMessage);
            }

            var element = this.WrappedElement;
            if (typeof(IWrapsElement).IsAssignableFrom(declaringType))
            {
                return new ReturnMessage(element, null, 0, methodCallMessage.LogicalCallContext, methodCallMessage);
            }

            return InvokeMethod(element, methodCallMessage);
        }
        public bool CanCastTo(Type fromType, object o)
        {
            return InterfacesToBeProxied.Contains(fromType);
        }
        public string TypeName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}