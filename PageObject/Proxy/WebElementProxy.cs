using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.PageObjects;
using WebDriverFramework.Elements;

namespace PageObject.Proxy
{
    public class WebElementProxy : DriverObjectProxy, IRemotingTypeInfo, ICacheable, IWrapsElement
    {
        private static List<Type> InterfacesToBeProxied => new List<Type>
        {
            typeof(ILocatable),
            typeof(IWebElement),
            typeof(IWrapsElement)
        };

        private IWebElement _cachedElement;

        public WebElementProxy(IWebElement element) : this(typeof(IWebElement), null, null, true)
        {
            this._cachedElement = element;
        }
        public WebElementProxy(IEnumerable<By> bys, IElementLocator locator, bool shouldCached = false)
            : this(typeof(IWebElement), locator, bys, shouldCached)
        {
        }
        public WebElementProxy(Type typeToBeProxied, IElementLocator locator, IEnumerable<By> bys, bool shouldCached)
            : base(typeToBeProxied, locator, bys, shouldCached)
        {
        }

        public bool IsCached => (_cachedElement as ICacheable)?.IsCached ?? _cachedElement is RemoteWebElement;
        public IWebElement WrappedElement
        {
            get
            {
                if (this._cachedElement != null)
                {
                    return this._cachedElement;
                }

                this.OnBeforeSearching();
                var element = this.Locator.LocateElement(this.Bys);
                if (this.ShouldCached)
                {
                    this._cachedElement = element;
                }

                return element;
            }
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCallMessage = (IMethodCallMessage)msg;
            var declaringType = ((MethodInfo)methodCallMessage.MethodBase).DeclaringType;

            var element = this.WrappedElement;
            if (typeof(IWrapsElement).IsAssignableFrom(declaringType))
            {
                return new ReturnMessage(element, null, 0, methodCallMessage.LogicalCallContext, methodCallMessage);
            }

            return InvokeMethod(element, methodCallMessage);
        }

        public bool CanCastTo(Type fromType, object o) => InterfacesToBeProxied.Contains(fromType);
        public string TypeName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }

    public class WebElementProxy1 : DriverObjectProxy, IRemotingTypeInfo, IWrapsElement
    {
        private static List<Type> InterfacesToBeProxied => new List<Type>
        {
            typeof(ILocatable),
            typeof(IWebElement),
            typeof(IWrapsElement)
        };

        private WebElement _cachedElement;

        public WebElementProxy1(WebElement element) : this(typeof(IWebElement), null, null, true)
        {
            this._cachedElement = element;
        }
        public WebElementProxy1(IEnumerable<By> bys, IElementLocator locator, bool shouldCached = false)
            : this(typeof(IWebElement), locator, bys, shouldCached)
        {
        }
        public WebElementProxy1(Type typeToBeProxied, IElementLocator locator, IEnumerable<By> bys, bool shouldCached)
            : base(typeToBeProxied, locator, bys, shouldCached)
        {
        }

       // public bool IsCached => (_cachedElement?.IsCached ?? _cachedElement.Element is RemoteWebElement;
        public IWebElement WrappedElement
        {
            get
            {
                if (this._cachedElement != null)
                {
                    return this._cachedElement.Element;
                }

                this.OnBeforeSearching();
                var element = this.Locator.LocateElement(this.Bys);
                if (this.ShouldCached)
                {
                  //  this._cachedElement = element;
                }

                return element;
            }
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCallMessage = (IMethodCallMessage)msg;
            var declaringType = ((MethodInfo)methodCallMessage.MethodBase).DeclaringType;

            var element = this.WrappedElement;
            if (typeof(IWrapsElement).IsAssignableFrom(declaringType))
            {
                return new ReturnMessage(element, null, 0, methodCallMessage.LogicalCallContext, methodCallMessage);
            }

            return InvokeMethod(element, methodCallMessage);
        }

        public bool CanCastTo(Type fromType, object o) => InterfacesToBeProxied.Contains(fromType);
        public string TypeName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}