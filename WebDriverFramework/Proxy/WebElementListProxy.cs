namespace WebDriverFramework.Proxy
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;

    /// <summary>
    /// Represents a proxy class for a list of elements to be used with the PageFactory.
    /// </summary>
    public class WebElementListProxy : DriverObjectProxy
    {
        private List<IWebElement> cachedElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebElementListProxy"/> class.
        /// </summary>
        /// <param name="typeToBeProxied">The <see cref="Type"/> of object for which to create a proxy.</param>
        /// <param name="locator">The <see cref="IElementLocator"/> implementation that
        /// determines how elements are located.</param>
        /// <param name="bys">The list of methods by which to search for the elements.</param>
        /// <param name="shouldCached"><see langword="true"/> to shouldCached the lookup to the element; otherwise, <see langword="false"/>.</param>
        public WebElementListProxy(Type typeToBeProxied, IElementLocator locator, IEnumerable<By> bys, bool shouldCached)
            : base(typeToBeProxied, locator, bys, shouldCached)
        {
        }

        /// <summary>
        /// Gets the list of IWebElement objects this proxy represents, returning a cached one if requested.
        /// </summary>
        private List<IWebElement> Elements
        {
            get
            {
                if (this.cachedElements != null)
                {
                    return this.cachedElements;
                }

                var elements = this.Locator.LocateElements(this.Bys).ToList();
                if (this.ShouldCached)
                {
                    this.cachedElements = elements;
                }

                return elements;
            }
        }

        /// <summary>
        /// Invokes the method that is specified in the provided <see cref="IMessage"/> on the
        /// object that is represented by the current instance.
        /// </summary>
        /// <param name="msg">An <see cref="IMessage"/> that contains an <see cref="IDictionary"/>  of
        /// information about the method call. </param>
        /// <returns>The message returned by the invoked method, containing the return value and any
        /// out or ref parameters.</returns>
        public override IMessage Invoke(IMessage msg)
        {
            var elements = this.Elements;
            return InvokeMethod(elements, msg as IMethodCallMessage);
        }
    }
}