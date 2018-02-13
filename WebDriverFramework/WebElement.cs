namespace WebDriverFramework
{
    using Extension;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.Extensions;
    using OpenQA.Selenium.Support.PageObjects;
    using Proxy;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class WebElement
    {
        private WebElementProxy _webElementProxy;
        private double _elementSearchTimeout;

        public WebElement(IWebElement implicitElement, IWebDriver driver) : this(implicitElement, null, driver)
        {
        }
        internal WebElement(IWebElement implicitElement, WebElement parent, IWebDriver driver) : this(driver)
        {
            this._webElementProxy = new WebElementProxy(implicitElement);
            this.Parent = parent;
        }

        public WebElement(By locator, IWebDriver driver) : this(locator, null, driver)
        {
        }
        public WebElement(By locator, WebElement parent, IWebDriver driver) : this(driver)
        {
            ISearchContext searchContext = parent?.WrappedElement ?? driver as ISearchContext;
            this._webElementProxy = new WebElementProxy(typeof(IWebElement), new DefaultElementLocator(searchContext),
                new[] { locator }, false);
            this.Parent = parent;
        }

        public WebElement(WebElementProxy webElementProxy, IWebDriver driver) : this(driver)
        {
            this._webElementProxy = webElementProxy;
        }
        private WebElement(IWebDriver driver)
        {
            this.WrappedDriver = driver;
        }

        public static double WaitTimeout { get; set; } = 60;

        public IWebDriver WrappedDriver { get; }
        public IWebElement WrappedElement => this._webElementProxy.IsImplicitSet ?
                                                this._webElementProxy.WrappedElement
                                                : (IWebElement)this._webElementProxy.GetTransparentProxy();

        public IWebElement Element => this.WrappedElement.Unwrap();

        public List<By> Locators => this._webElementProxy.Bys.ToList();
        public By Locator => Locators.First();
        public WebElement Parent { get; }

        public bool IsCached => this._webElementProxy.IsCached;

        public bool Exist
        {
            get
            {
                try
                {
                    CheckStaleness();
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

        public WebElement CheckStaleness()
        {
            StubActionOnElement();
            return this;
        }
        public WebElement StubActionOnElement()
        {
            //call any property on element
            var tagName = Element.TagName;
            return this;
        }

        public WebElement Get(string xpath)
        {
            return Get(By.XPath(xpath));
        }
        public WebElement Get(By locator)
        {
            return new WebElement(locator, this, this.WrappedDriver);
        }
        public ListWebElement GetAll(By locator)
        {
            return new ListWebElement(locator, this, this.WrappedDriver);
        }
        public ListWebElement GetAll(string xpath)
        {
            return GetAll(By.XPath(xpath));
        }
        public WebElement Locate()
        {
            return new WebElement(this.Element, this.WrappedDriver);
        }

        #region JS
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
            var elem = this.Element;
            this.WrappedDriver.ExecuteJavaScript($"window.scrollTo({elem.Location.X}, {elem.Location.Y})",
                this.Element);
        }
        #endregion

        #region MyRegion
        public bool Displayed => Element.Displayed;
        public string Text => TagName == "input" ? GetAttribute("value") : Element.Text;
        public bool Enabled => Element.Enabled;
        public bool Selected => Element.Selected;
        public string TagName => Element.TagName;
        public Point Location => Element.Location;
        public Size Size => Element.Size;

        public WebElement Clear()
        {
            Element.Clear();
            return this;
        }
        public WebElement SendKeys(string text)
        {
            Element.SendKeys(text);
            return this;
        }
        public WebElement Submit()
        {
            Element.Submit();
            return this;
        }
        public WebElement Click()
        {
            Element.Click();
            return this;
        }
        public string GetAttribute(string attributeName)
        {
            return Element.GetAttribute(attributeName);
        }
        public string GetProperty(string propertyName)
        {
            return Element.GetProperty(propertyName);
        }
        public string GetCssValue(string propertyName)
        {
            return Element.GetCssValue(propertyName);
        }
        #endregion

        #region Wait
        public T Wait<T>(Func<WebElement, T> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            timeout = ResolveTime(timeout, WaitTimeout);
            return this.WrappedDriver.Wait(d => condition(this), timeout, exceptionTypes);
        }
        public bool TryWait(Func<WebElement, bool> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            try
            {
                return this.Wait(condition, timeout, exceptionTypes);
            }
            catch (WebDriverTimeoutException)
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

        private static double ResolveTime(double time, double currentValue) => time < 0 ? currentValue : time;
    }
}
