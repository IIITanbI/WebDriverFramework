namespace WebDriverFramework.Proxy
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;

    /// <summary>
    /// Represents a base proxy class for objects used with the PageFactory.
    /// </summary>
    public abstract class DriverObjectProxy : RealProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverObjectProxy"/> class.
        /// </summary>
        /// <param name="classToProxy">The <see cref="Type"/> of object for which to create a proxy.</param>
        /// <param name="locator">The <see cref="IElementLocator"/> implementation that
        /// determines how elements are located.</param>
        /// <param name="bys">The list of methods by which to search for the elements.</param>
        /// <param name="shouldCached"><see langword="true"/> to shouldCached the lookup to the element; otherwise, <see langword="false"/>.</param>
        protected DriverObjectProxy(Type classToProxy, IElementLocator locator, IEnumerable<By> bys, bool shouldCached) : base(classToProxy)
        {
            this.Locator = locator;
            this.Bys = bys;
            this.ShouldCached = shouldCached;
        }

        public event EventHandler BeforeSearching;

        /// <summary>
        /// Gets the <see cref="IElementLocator"/> implementation that determines how elements are located.
        /// </summary>
        public IElementLocator Locator { get; set; }

        /// <summary>
        /// Gets the list of methods by which to search for the elements.
        /// </summary>
        public IEnumerable<By> Bys { get; set; }

        /// <summary>
        /// Gets a value indicating whether element search results should be cached.
        /// </summary>
        public bool ShouldCached { get; set; }

        /// <summary>
        /// Invokes a method on the object this proxy represents.
        /// </summary>
        /// <param name="msg">Message containing the parameters of the method being invoked.</param>
        /// <param name="representedValue">The object this proxy represents.</param>
        /// <returns>The <see cref="ReturnMessage"/> instance as a result of method invocation on the
        /// object which this proxy represents.</returns>
        protected static ReturnMessage InvokeMethod(object representedValue, IMethodCallMessage msg)
        {
            var proxiedMethod = (MethodInfo)msg.MethodBase;
            return new ReturnMessage(proxiedMethod.Invoke(representedValue, msg.Args), null, 0, msg.LogicalCallContext, msg);
        }

        protected virtual void OnBeforeSearching()
        {
            BeforeSearching?.Invoke(this, EventArgs.Empty);
        }
    }
}