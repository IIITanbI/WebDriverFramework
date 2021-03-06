﻿namespace WebDriverFramework.Proxy
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Internal;
    using OpenQA.Selenium.Support.PageObjects;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Messaging;

    public class WebElementProxy : DriverObjectProxy, IWrapsElement, IRemotingTypeInfo
    {
        private static List<Type> InterfacesToBeProxied => new List<Type>
        {
            typeof(IWebElement),
            typeof(ILocatable),
            typeof(IWrapsElement),
        };

        private IWebElement cachedElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebElementProxy"/> class.
        /// </summary>
        /// <param name="classToProxy">The <see cref="Type"/> of object for which to create a proxy.</param>
        /// <param name="typeToBeProxied"></param>
        /// <param name="locator">The <see cref="IElementLocator"/> implementation that determines
        /// how elements are located.</param>
        /// <param name="bys">The list of methods by which to search for the elements.</param>
        /// <param name="shouldCached"><see langword="true"/> to shouldCached the lookup to the element; otherwise, <see langword="false"/>.</param>
        public WebElementProxy(Type typeToBeProxied, IElementLocator locator, IEnumerable<By> bys, bool shouldCached)
            : base(typeToBeProxied, locator, bys, shouldCached)
        {
        }

        public WebElementProxy(IWebElement element) : base(typeof(IWebElement), null, null, true)
        {
            this.cachedElement = element;
        }

        public bool IsCached => cachedElement != null;

        /// <summary>
        /// Gets the <see cref="IWebElement"/> wrapped by this object.
        /// </summary>
        public IWebElement WrappedElement
        {
            get
            {
                if (this.cachedElement != null)
                {
                    return this.cachedElement;
                }

                var e = this.Locator.LocateElement(this.Bys);
                if (this.ShouldCached)
                {
                    this.cachedElement = e;
                }

                return e;
            }
        }

        /// <summary>
        /// Invokes the method that is specified in the provided <see cref="IMessage"/> on the
        /// object that is represented by the current instance.
        /// </summary>
        /// <param name="msg">An <see cref="IMessage"/> that contains a dictionary of
        /// information about the method call. </param>
        /// <returns>The message returned by the invoked method, containing the return value and any
        /// out or ref parameters.</returns>
        public override IMessage Invoke(IMessage msg)
        {
            var element = this.WrappedElement;
            IMethodCallMessage methodCallMessage = msg as IMethodCallMessage;

            if (typeof(IWrapsElement).IsAssignableFrom((methodCallMessage.MethodBase as MethodInfo).DeclaringType))
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