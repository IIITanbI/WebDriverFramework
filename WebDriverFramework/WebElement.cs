namespace WebDriverFramework
{
    using Extension;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Internal;
    using OpenQA.Selenium.Support.Extensions;
    using OpenQA.Selenium.Support.PageObjects;
    using Proxy;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;

    public class WebElement : IWebElement, IWrapsDriver, IWrapsElement
    {
        private WebElementProxy WebElementProxy;
        private double _elementSearchTimeout;

        public WebElement(IWebElement implicitElement, IWebDriver driver) : this(implicitElement, null, driver)
        {
        }
        internal WebElement(IWebElement implicitElement, WebElement parent, IWebDriver driver) : this(driver)
        {
            this.WebElementProxy = new WebElementProxy(implicitElement);
            this.Parent = parent;
        }

        public WebElement(By locator, IWebDriver driver) : this(locator, null, driver)
        {
        }
        public WebElement(By locator, WebElement parent, IWebDriver driver) : this(driver)
        {
            ISearchContext searchContext = parent?.WrappedElement ?? driver as ISearchContext;
            this.WebElementProxy = new WebElementProxy(typeof(IWebElement), new DefaultElementLocator(searchContext), new[] { locator }, false);
            this.Parent = parent;
        }

        public WebElement(WebElementProxy webElementProxy, IWebDriver driver) : this(driver)
        {
            this.WebElementProxy = webElementProxy;
        }
        private WebElement(IWebDriver driver)
        {
            this.WrappedDriver = driver;
            this.SetSearchElementTimeout(DefaultElementSearchTimeout);
        }

        public static double DefaultElementSearchTimeout { get; set; } = 30;
        public double ElementSearchTimeout
        {
            get => _elementSearchTimeout;
            set => _elementSearchTimeout = ResolveTime(value, DefaultElementSearchTimeout);
        }
        public double WaitTimeout { get; set; } = 60;

        public IWebDriver WrappedDriver { get; }
        public IWebElement WrappedElement => (IWebElement)this.WebElementProxy.GetTransparentProxy();
        public IWebElement Element => this.WrappedDriver.Wait(() => this.WrappedElement.Unwrap(), this.ElementSearchTimeout);

        public List<By> Locators => this.WebElementProxy.Bys.ToList();
        public By Locator => Locators.First();
        public WebElement Parent { get; }

        public bool IsCached => this.WebElementProxy.IsCached;
        public bool Exist
        {
            get
            {
                try
                {
                    StubActionOnElement();
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
        public bool IsStale
        {
            get
            {
                try
                {
                    StubActionOnElement();
                    return true;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    return true;
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
            return Get(xpath, DefaultElementSearchTimeout);
        }
        public WebElement Get(string xpath, double searchTimeout)
        {
            return Get(By.XPath(xpath), searchTimeout);
        }
        public WebElement Get(By locator)
        {
            return Get(locator, DefaultElementSearchTimeout);
        }
        public WebElement Get(By locator, double searchTimeout)
        {
            return new WebElement(locator, this, this.WrappedDriver).SetSearchElementTimeout(searchTimeout);
        }

        private WebElement Get(IWebElement implicitElement)
        {
            return new WebElement(implicitElement, this, this.WrappedDriver);
        }

        public IElementList GetAll(By locator)
        {
            return new ListWebElement(locator, this, this.WrappedDriver);
        }
        public IElementList GetAll(string xpath)
        {
            return GetAll(By.XPath(xpath));
        }

        public WebElement Locate()
        {
            return this.IsCached ? this : new WebElement(this.Element, this.WrappedDriver);
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
            this.WrappedDriver.ExecuteJavaScript($"window.scrollTo({elem.Location.X}, {elem.Location.Y})", this.Element);
        }
        #endregion

        #region MyRegion
        public string TagName => Element.TagName;
        public string Text => TagName == "input" ? GetAttribute("value") : Element.Text;
        public bool Enabled => Element.Enabled;
        public bool Selected => Element.Selected;
        public Point Location => Element.Location;
        public Size Size => Element.Size;
        public bool Displayed => Element.Displayed;

        public void Clear()
        {
            Element.Clear();
        }
        public void SendKeys(string text)
        {
            Element.SendKeys(text);
        }
        public void Submit()
        {
            Element.Submit();
        }
        public void Click()
        {
            Element.Click();
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

        public IWebElement FindElement(By by)
        {
            return this.Get(by).Locate();
        }
        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return Element.FindElements(by).Select(this.Get).Cast<IWebElement>().ToList().AsReadOnly();
        }
        #endregion

        #region Wait
        public T Wait<T>(Func<T> condition, double timeout, params Type[] exceptionTypes)
        {
            timeout = ResolveTime(timeout, this.WaitTimeout);
            return this.WrappedDriver.GetWait(timeout, exceptionTypes).Until(condition);
        }
        public T Wait<T>(Func<IWebDriver, T> condition, double timeout, params Type[] exceptionTypes)
        {
            timeout = ResolveTime(timeout, this.WaitTimeout);
            return this.WrappedDriver.GetWait(timeout, exceptionTypes).Until(condition);
        }

        public WebElement WaitForPresent(double timeout = -1)
        {
            Wait(() =>
            {
                try
                {
                    this.StubActionOnElement();
                    return true;
                }
                catch (StaleElementReferenceException) when (!this.IsCached)
                {
                    return false;
                }
            }, timeout);
            return this;
        }
        public WebElement WaitForNotPresent(double timeout = -1)
        {
            Wait(() =>
            {
                try
                {
                    this.StubActionOnElement();
                    return true;
                }
                catch (NoSuchElementException)
                {
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    return true;
                }
            }, timeout);

            return this;
        }

        public WebElement WaitForElementDisplayed(double timeout = -1)
        {
            Wait(() =>
            {
                try
                {
                    return this.Element.Displayed;
                }
                catch (StaleElementReferenceException) when (!this.IsCached)
                {
                    return false;
                }
            }, timeout);

            return this;
        }
        public WebElement WaitForElementNotDisplayed(double timeout = -1)
        {
            Wait(() =>
            {
                try
                {
                    return !this.Element.Displayed;
                }
                catch (NoSuchElementException)
                {
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    return true;
                }
            }, timeout);

            return this;
        }

        public WebElement WaitForElementClickable(double timeout = -1)
        {
            Wait(() =>
            {
                try
                {
                    return this.Element.Enabled;
                }
                catch (StaleElementReferenceException) when (!this.IsCached)
                {
                    return false;
                }
            }, timeout);

            return this;
        }

        private bool? ProcessException(Exception ex, bool staleElementReferenceException, bool noSuchElementException = false)
        {
            switch (ex)
            {
                case StaleElementReferenceException s when staleElementReferenceException || !this.IsCached:
                    return staleElementReferenceException;
                case NoSuchElementException n:
                    return noSuchElementException;
            }

            return null;
        }
        #endregion

        public WebElement SetSearchElementTimeout(double timeout)
        {
            this.ElementSearchTimeout = timeout;
            return this;
        }
        private double ResolveTime(double time, double defaultValue) => time < 0 ? defaultValue : time;
    }
}
