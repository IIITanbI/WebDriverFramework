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
    internal interface ICacheable
    {
        bool IsCached { get; }
    }

    public class WebElementProxy : DriverObjectProxy, IRemotingTypeInfo, ICacheable, IWrapsElement
    {
        private static List<Type> InterfacesToBeProxied => new List<Type>
        {
            typeof(ILocatable),
            typeof(IWebElement),
            typeof(IWrapsElement)
        };

        private IWebElement cachedElement;

        public WebElementProxy(IWebElement element) : this(typeof(IWebElement), null, null, true)
        {
            this.cachedElement = element;
        }
        public WebElementProxy(IEnumerable<By> bys, IElementLocator locator, bool shouldCached = false)
            : this(typeof(IWebElement), locator, bys, shouldCached)
        {
        }
        public WebElementProxy(Type typeToBeProxied, IElementLocator locator, IEnumerable<By> bys, bool shouldCached)
            : base(typeToBeProxied, locator, bys, shouldCached)
        {
        }

        public bool IsCached => (cachedElement as ICacheable)?.IsCached ?? cachedElement is RemoteWebElement;
        public IWebElement WrappedElement
        {
            get
            {
                if (this.cachedElement != null)
                {
                    return this.cachedElement;
                }

                this.FrameSwitcher?.Invoke();
                var element = this.Locator.LocateElement(this.Bys);
                if (this.ShouldCached)
                {
                    this.cachedElement = element;
                }

                return element;
            }
        }

        public Action FrameSwitcher { get; set; }

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage methodCallMessage = msg as IMethodCallMessage;

            var declaringType = (methodCallMessage.MethodBase as MethodInfo).DeclaringType;

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