namespace WebDriverFramework.Elements
{
    using OpenQA.Selenium;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public abstract partial class WebElement : IMyWebElement, IGetElement, IGetElements
    {
        public static double DefaultWaitTimeout { get; set; } = 60;

        private IMyWebElement _parent;
        private readonly IWebElement _implicitElement;

        protected WebElement(IWebElement implicitElement, WebDriver driver)
        {
            this._implicitElement = implicitElement;
            this.Driver = driver;
        }
        protected WebElement(By locator, IMyWebElement parent, WebDriver driver = null)
        {
            this.Locator = locator;
            this._parent = parent;
            this.Driver = parent != null ? parent.Driver : driver;
        }

        public WebDriver Driver { get; }
        public By Locator { get; }
        public IMyWebElement Parent
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
        public IList<IMyWebElement> Parents
        {
            get
            {
                var parent = this.Parent;
                var parents = new List<IMyWebElement>();
                while (parent != null)
                {
                    parents.Add(parent);
                    parent = parent.Parent;
                }
                return parents;
            }
        }
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
                return element;
            }
        }

        private ISearchContext ParentContext => this.Parent != null ? this.Parent.Element : (ISearchContext)this.Driver.NativeDriver;

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
        public IEnumerable<T> GetAll<T>(By locator) => new FindAllHelper<T>(this, locator);

        protected void StubAction()
        {
            //just stub action
            if (this.TagName == null)
            {
                throw new NotImplementedException();
            }
        }

        protected void SwitchToParentFrame()
        {
            var parentFrame = this.Parents.OfType<IFrameElement>().LastOrDefault();
            if (parentFrame == null)
            {
                this.Driver.SwitchToDefaultContent();
            }
            else
            {
                this.Driver.SwitchToFrame(parentFrame);
            }
        }
    }
}