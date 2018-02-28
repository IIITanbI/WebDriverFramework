namespace WebDriverFramework.Elements
{
    using Extension;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Internal;
    using OpenQA.Selenium.Support.Extensions;
    using OpenQA.Selenium.Support.PageObjects;
    using Proxy;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public interface IBaseWebElement
    {
        IWebDriver WrappedDriver { get; }
        IWebElement ProxyElement { get; }
        IWebElement Element { get; }
        By Locator { get; }
    }
    public interface ILocate<out T>
    {
        T Locate();
    }
    public interface IFindElement<T> where T : ILocate<T>
    {
        T Find(string xpath);
        T Find(By locator);
        IList<T> FindAll(string xpath = ".//*");
        IList<T> FindAll(By locator);
        T Get(string xpath);
        T Get(By locator);
        IEnumerable<T> GetAll(string xpath);
        IEnumerable<T> GetAll(By locator);
    }
    public interface IFindElement
    {
        T Find<T>(string xpath) where T : ILocate<T>;
        T Find<T>(By locator) where T : ILocate<T>;
        IList<T> FindAll<T>(string xpath = ".//*") where T : ILocate<T>;
        IList<T> FindAll<T>(By locator) where T : ILocate<T>;
        T Get<T>(string xpath);
        T Get<T>(By locator);
        IEnumerable<T> GetAll<T>(string xpath);
        IEnumerable<T> GetAll<T>(By locator);
    }

    public abstract partial class WebElement : IBaseWebElement, IFindElement, IFindElement<LabelElement>
    {
        public static double DefaultWaitTimeout { get; set; } = 60;

        private IBaseWebElement _parent;

        protected WebElement(IWebElement implicitElement, IWebDriver driver)
        {
            this.WrappedDriver = driver;
            this.WebElementProxy = new WebElementProxy(implicitElement);
        }
        protected WebElement(By locator, WebElement parent, IWebDriver driver = null)
        {
            this.WebElementProxy = new WebElementProxy(new[] { locator }, null);
            this.WrappedDriver = driver;
            this.Parent = parent;
        }

        //public WebElement(By locator, WebElement parent, IWebElement implicitElement = null, IWebDriver driver = null)
        //{
        //    this.WrappedDriver = driver;

        //    if (implicitElement != null)
        //    {
        //        this.WebElementProxy = new WebElementProxy(implicitElement);
        //    }
        //    else
        //    {
        //        this.WebElementProxy = new WebElementProxy(new[] { locator }, null);
        //        this.Parent = parent;
        //    }
        //}

        protected WebElementProxy WebElementProxy { get; }
        public IWebDriver WrappedDriver { get; }
        public IWebElement ProxyElement => (IWebElement)this.WebElementProxy.GetTransparentProxy();
        public IWebElement Element => this.UnwrapProxy();

        public By Locator => this.WebElementProxy.Bys.FirstOrDefault();
        public IBaseWebElement Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                this.WebElementProxy.Locator = this.GetParentElementLocator();
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

        #region Find elements
        public T Find<T>(string xpath) where T : ILocate<T> => Find<T>(By.XPath(xpath));
        public T Find<T>(By locator) where T : ILocate<T> => ElementFactory.Create<T>(locator, this).Locate();
        public IList<T> FindAll<T>(string xpath = ".//*") where T : ILocate<T> => FindAll<T>(By.XPath(xpath));
        public IList<T> FindAll<T>(By locator) where T : ILocate<T> => GetAll<T>(locator).LocateAll().ToList();

        public T Get<T>(string xpath) => Get<T>(By.XPath(xpath));
        public T Get<T>(By locator) => ElementFactory.Create<T>(locator, this);
        public IEnumerable<T> GetAll<T>(string xpath) => GetAll<T>(By.XPath(xpath));
        public IEnumerable<T> GetAll<T>(By locator) => this.WrappedDriver.FindElements(locator).Select(e => ElementFactory.Create<T>(e, this.WrappedDriver));

        public LabelElement Find(string xpath) => Find<LabelElement>(xpath);
        public LabelElement Find(By locator) => Find<LabelElement>(locator);
        public IList<LabelElement> FindAll(string xpath = ".//*") => FindAll<LabelElement>(xpath);
        public IList<LabelElement> FindAll(By locator) => FindAll<LabelElement>(locator);

        public LabelElement Get(string xpath) => Get<LabelElement>(xpath);
        public LabelElement Get(By locator) => Get<LabelElement>(locator);
        public IEnumerable<LabelElement> GetAll(string xpath) => GetAll<LabelElement>(xpath);
        public IEnumerable<LabelElement> GetAll(By locator) => GetAll<LabelElement>(locator); 
        #endregion

        protected IElementLocator GetParentElementLocator()
        {
            return new DefaultElementLocator(this.Parent != null ? this.Parent.ProxyElement : (ISearchContext)this.WrappedDriver);
        }
        protected void StubAction()
        {
            //just stub acition
            if (this.TagName == null)
            {
                throw new NotImplementedException();
            }
        }

        private IWebElement UnwrapProxy()
        {
            IWebElement element = this.ProxyElement;
            while (element is IWrapsElement wrap)
            {
                element = wrap.WrappedElement;
            }

            return element;
        }
    }

    public abstract partial class WebElement
    {
        public string JsText => this.WrappedDriver.ExecuteJavaScript<string>("arguments[0].text", this.Element);

        public void JSClick()
        {
            this.WrappedDriver.ExecuteJavaScript("arguments[0].click()", this.Element);
        }
        public void JSScrollIntoView()
        {
            this.WrappedDriver.ExecuteJavaScript("arguments[0].scrollIntoView(true)", this.Element);
        }
        public void JSScrollTo()
        {
            this.WrappedDriver.ExecuteJavaScript($"window.scrollTo({this.Element.Location.X}, {this.Element.Location.Y})", this.Element);
        }
    }
}