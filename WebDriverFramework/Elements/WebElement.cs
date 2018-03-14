namespace WebDriverFramework.Elements
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Internal;
    using OpenQA.Selenium.Support.PageObjects;
    using Proxy;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public abstract partial class WebElement : IFindElement
    {
        private WebElement _parent;
        private readonly WebElementProxy _webElementProxy;

        public static double DefaultWaitTimeout { get; set; } = 60;

        protected WebElement(IWebElement implicitElement, WebDriver driver)
        {
            this.Driver = driver;
            this._webElementProxy = new WebElementProxy(implicitElement);
        }
        protected WebElement(By locator, WebElement parent, WebDriver driver = null)
        {
            this._parent = parent;
            this.Driver = parent != null ? parent.Driver : driver;

            this._webElementProxy = new WebElementProxy(new[] { locator }, this.GetParentElementLocator());
            this._webElementProxy.BeforeSearching += (o, e) => SwitchFrames(this.AllParents);
        }

        public WebDriver Driver { get; }
        public IWebElement ProxyElement => (IWebElement)this._webElementProxy.GetTransparentProxy();
        public virtual IWebElement Element => this.UnwrapProxy();

        protected IList<WebElement> AllParents
        {
            get
            {
                var parents = this.Parent?.AllParents;
                parents?.Add(this.Parent);
                return parents ?? new List<WebElement>();
            }
        }
        public By Locator => this._webElementProxy.Bys.FirstOrDefault();
        public WebElement Parent
        {
            get => this._parent;
            set
            {
                if (this._webElementProxy.IsCached)
                {
                    throw new Exception("Already cached. Do not need to change parent");
                }

                this._parent = value;
                this._webElementProxy.Locator = GetParentElementLocator();
            }
        }

        #region Element properties
        public bool Exist
        {
            get
            {
                try
                {
                    this.StubAction();
                    return true;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            }
        }
        public bool Displayed => Element.Displayed;

        public string TagName => Element.TagName;
        public Point Location => Element.Location;
        public Size Size => Element.Size;

        public string GetAttribute(string attributeName) => Element.GetAttribute(attributeName);
        public string GetProperty(string propertyName) => Element.GetProperty(propertyName);
        public string GetCssValue(string propertyName) => Element.GetCssValue(propertyName);
        #endregion

        public void Click()
        {
            this.Element.Click();
        }

        public T Get<T>(By locator) => ElementFactory.Create<T>(locator, this, null);
        public IEnumerable<T> GetAll<T>(By locator)
        {
            var proxy = new WebElementListProxy(new[] { locator }, this.GetElementLocator(), false);
            proxy.BeforeSearching += (o, e) => SwitchFrames(this.AllParents.Concat(new[] { this }));

            return proxy.Elements.Select(e => ElementFactory.Create<T>(e, this.Driver));
        }

        protected void StubAction()
        {
            //just stub action
            if (this.TagName == null)
            {
                throw new NotImplementedException();
            }
        }
        protected IElementLocator GetParentElementLocator()
        {
            ISearchContext context = this.Driver.Driver;
            if (this.Parent != null && !(this.Parent is IFrameElement))
            {
                context = this.Parent.ProxyElement;
            }

            return new DefaultElementLocator(context);
        }
        protected IElementLocator GetElementLocator() => new DefaultElementLocator(this.ProxyElement);

        protected IWebElement UnwrapProxy()
        {
            var element = this.ProxyElement;
            while (element is IWrapsElement wrap)
            {
                element = wrap.WrappedElement;
            }

            return element;
        }
        protected void SwitchFrames(IEnumerable<WebElement> elements)
        {
            var parentFrame = elements.LastOrDefault(p => p is IFrameElement);
            if (parentFrame == null)
            {
                this.Driver.Driver.SwitchTo().DefaultContent();
            }
            else
            {
                this.Driver.Driver.SwitchTo().Frame(parentFrame.Element);
            }
        }
    }
}