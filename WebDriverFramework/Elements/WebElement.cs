using System.Collections.ObjectModel;

namespace WebDriverFramework
{
    using Extension;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using Proxy;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public interface ISearchContext<T>
    {
        T FindElement(By by);
        ReadOnlyCollection<T> FindElements(By by);
    }

    public class WebElement : IWebElement
    {
        public static double DefaultWaitTimeout { get; set; } = 60;

        private WebElement _parent;

        public WebElement(IWebElement implicitElement, IWebDriver driver) : this(new WebElementProxy(implicitElement), null, driver)
        {
        }
        public WebElement(By locator, IWebDriver driver) : this(new[] { locator }, driver)
        {
        }
        public WebElement(IEnumerable<By> locators, IWebDriver driver) : this(new WebElementProxy(locators, driver), null, driver)
        {
        }
        public WebElement(By locator, WebElement parent) : this(new WebElementProxy(new[] { locator }, parent.WrappedElement), parent, parent.WrappedDriver)
        {
        }

        private WebElement(WebElementProxy webElementProxy, WebElement parent, IWebDriver driver)
        {
            this.WebElementProxy = webElementProxy;
            this.WrappedDriver = driver;
            this.Parent = parent;
        }

        protected WebElementProxy WebElementProxy { get; }
        public IWebDriver WrappedDriver { get; }
        public IWebElement WrappedElement => (IWebElement)this.WebElementProxy.GetTransparentProxy();
        public IWebElement Element => this.WrappedElement.Unwrap();

        public List<By> Locators => this.WebElementProxy.Bys.ToList();
        public By Locator => this.Locators.FirstOrDefault();
        public WebElement Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                this.WebElementProxy.Locator = this.GetParentElementLocator();
            }
        }

        public bool IsCached => this.WebElementProxy.IsCached;
        public bool ShouldCached
        {
            get => this.WebElementProxy.ShouldCached;
            set => this.WebElementProxy.ShouldCached = value;
        }

        public bool Exist => this.Element.Exist();

        public WebElement Find(string xpath) => Find(By.XPath(xpath));
        public WebElement Find(By locator) => Get(locator).Locate();
        public ListWebElement FindAll(string xpath = ".//*") => FindAll(By.XPath(xpath));
        public ListWebElement FindAll(By locator) => GetAll(locator).Locate();

        public WebElement Get(string xpath) => Get(By.XPath(xpath));
        public WebElement Get(By locator) => new WebElement(locator, this);
        public ListWebElement GetAll(string xpath = ".//*") => GetAll(By.XPath(xpath));
        public ListWebElement GetAll(By locator) => new ListWebElement(locator, this);

        public WebElement Locate() => new WebElement(this.Element, this.WrappedDriver);

        #region MyRegion
        public bool Displayed => Element.Displayed;
        public string Text => TagName == "input" ? GetAttribute("value") : Element.Text;
        public bool Enabled => Element.Enabled;
        public bool Selected => Element.Selected;
        public string TagName => Element.TagName;
        public Point Location => Element.Location;
        public Size Size => Element.Size;

        void IWebElement.SendKeys(string text) => this.Element.SendKeys(text);
        void IWebElement.Submit() => this.Element.Submit();
        void IWebElement.Click() => this.Element.Click();
        void IWebElement.Clear() => this.Element.Clear();

        public WebElement Clear()
        {
            ((IWebElement)this).Clear();
            return this;
        }
        public WebElement SendKeys(string text)
        {
            ((IWebElement)this).SendKeys(text);
            return this;
        }
        public WebElement Submit()
        {
            ((IWebElement)this).Submit();
            return this;
        }
        public WebElement Click()
        {
            ((IWebElement)this).Click();
            return this;
        }

        public string GetAttribute(string attributeName) => Element.GetAttribute(attributeName);
        public string GetProperty(string propertyName) => Element.GetProperty(propertyName);
        public string GetCssValue(string propertyName) => Element.GetCssValue(propertyName);

        public IWebElement FindElement(By by) => this.Find(by);
        public ReadOnlyCollection<IWebElement> FindElements(By by) => this.FindAll(by).Elements.Cast<IWebElement>().ToList().AsReadOnly();
        #endregion

        #region Wait
        public T Wait<T>(Func<WebElement, T> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            //this.WrappedDriver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
            timeout = timeout < 0 ? DefaultWaitTimeout : timeout;
            return this.WrappedDriver.Wait(d => condition(this), timeout, exceptionTypes);
        }
        public bool TryWait(Func<WebElement, bool> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            try
            {
                return this.Wait(condition, timeout, exceptionTypes);
            }
            catch
            {
                return false;
            }
        }
        public WebElement WaitUntil(Func<WebElement, bool> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            this.Wait(condition, timeout, exceptionTypes);
            return this;
        }

        public bool ShouldBe(Func<WebElement, bool> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            return this.TryWait(condition, timeout, exceptionTypes);
        }
        public bool ShouldNot(Func<WebElement, bool> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            return this.TryWait(e => !condition(e), timeout, exceptionTypes);
        }
        #endregion

        protected IElementLocator GetParentElementLocator()
        {
            return new DefaultElementLocator(this.Parent != null ? this.Parent.WrappedElement : (ISearchContext)this.WrappedDriver);
        }
    }
}
