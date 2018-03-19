namespace WebDriverFramework.Elements
{
    using OpenQA.Selenium;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public abstract partial class WebElement : IGetElement, IGetElements
    {
        private WebElement _parent;

        public static double DefaultWaitTimeout { get; set; } = 60;

        protected WebElement(IWebElement implicitElement, WebDriver driver)
        {
            this._implicitElement = implicitElement;
            this.Driver = driver;
        }
        protected WebElement(By locator, WebElement parent, WebDriver driver = null)
        {
            this.Locator = locator;
            this._parent = parent;
            this.Driver = parent != null ? parent.Driver : driver;
        }

        public WebDriver Driver { get; }

        public bool ShouldCached { get; set; }

        private IWebElement _implicitElement;

        private ISearchContext ParentContext => this.Parent != null ? this.Parent.Element : (ISearchContext)this.Driver.NativeDriver;

        public virtual IWebElement Element
        {
            get
            {
                if (this._implicitElement != null)
                {
                    return this._implicitElement;
                }

                this.SwitchToParentFrame();
                var element = this.ParentContext.FindElement(this.Locator);
                if (this.ShouldCached)
                {
                    this._implicitElement = element;
                }

                return element;
            }
        }

        protected IList<WebElement> AllParents
        {
            get
            {
                var parents = this.Parent?.AllParents;
                parents?.Add(this.Parent);
                return parents ?? new List<WebElement>();
            }
        }
        public By Locator { get; }

        public WebElement Parent
        {
            get => this._parent;
            set
            {
                if (this._implicitElement != null)
                {
                    throw new Exception("Already cached. Do not need to change parent");
                }

                this._parent = value;
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
        public IEnumerable<T> GetAll<T>(By locator) => this.Element.FindElements(locator).Select(e => ElementFactory.Create<T>(e, this.Driver));

        protected void StubAction()
        {
            //just stub action
            if (this.TagName == null)
            {
                throw new NotImplementedException();
            }
        }

        //protected IWebElement UnwrapProxy()
        //{
        //    var element = this.ProxyElement;
        //    while (element is IWrapsElement wrap)
        //    {
        //        element = wrap.WrappedElement;
        //    }

        //    return element;
        //}

        protected void SwitchToParentFrame()
        {
            var parentFrame = this.AllParents.OfType<IFrameElement>().LastOrDefault();
            if (parentFrame == null)
            {
                this.Driver.NativeDriver.SwitchTo().DefaultContent();
            }
            else
            {
                this.Driver.NativeDriver.SwitchTo().Frame(parentFrame.Element);
            }
        }
    }
}