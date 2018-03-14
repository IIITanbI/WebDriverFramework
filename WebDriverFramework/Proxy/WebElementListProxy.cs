namespace WebDriverFramework.Proxy
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;

    public class WebElementListProxy : DriverObjectProxy
    {
        private List<IWebElement> _cachedElements;

        public WebElementListProxy(List<IWebElement> elements) : this(null, null, false)
        {
            this._cachedElements = elements;
        }
        public WebElementListProxy(IEnumerable<By> bys, IElementLocator locator, bool shouldCached = false)
            : this(typeof(IList<IWebElement>), locator, bys, shouldCached)
        {
        }
        public WebElementListProxy(Type typeToBeProxied, IElementLocator locator, IEnumerable<By> bys, bool shouldCached)
            : base(typeToBeProxied, locator, bys, shouldCached)
        {
        }

        /// <summary>
        /// Gets the list of IWebElement objects this proxy represents, returning a cached one if requested.
        /// </summary>
        public List<IWebElement> Elements
        {
            get
            {
                if (this._cachedElements != null)
                {
                    return this._cachedElements.ToList();
                }

                this.OnBeforeSearching();
                var elements = this.Locator.LocateElements(this.Bys).ToList();
                if (this.ShouldCached)
                {
                    this._cachedElements = elements;
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