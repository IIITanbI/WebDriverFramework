using OpenQA.Selenium.Support.PageObjects;

namespace WebDriverFramework
{
    using Extension;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.Extensions;
    using Proxy;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class WebElement
    {
        public static double DefaultWaitTimeout { get; set; } = 60;

        private WebElementProxy _webElementProxy;

        public WebElement(IWebElement implicitElement, IWebDriver driver) : this(new WebElementProxy(implicitElement), driver)
        {
        }
        public WebElement(IWebElement implicitElement, WebElement parent) : this(new WebElementProxy(implicitElement), parent)
        {
        }

        public WebElement(By locator, IWebDriver driver) : this(new WebElementProxy(driver, locator), driver)
        {
        }
        public WebElement(By locator, WebElement parent) : this(new WebElementProxy(parent.WrappedElement, locator), parent)
        {
        }

        public WebElement(WebElementProxy webElementProxy, IWebDriver driver) : this(webElementProxy, null, driver)
        {
        }
        public WebElement(WebElementProxy webElementProxy, WebElement parent) : this(webElementProxy, parent, parent.WrappedDriver)
        {
        }

        private WebElement(WebElementProxy webElementProxy, WebElement parent, IWebDriver driver)
        {
            this._webElementProxy = webElementProxy;
            this.Parent = parent;
            this.WrappedDriver = driver;
        }

        public IWebDriver WrappedDriver { get; }
        public IWebElement WrappedElement => (IWebElement)this._webElementProxy.GetTransparentProxy();
        public IWebElement Element => this.WrappedElement.Unwrap();

        public List<By> Locators => this._webElementProxy.Bys?.ToList();
        public By Locator => Locators.FirstOrDefault();
        public WebElement Parent { get; }

        public bool IsCached => this._webElementProxy.IsCached;
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
            return new WebElement(locator, this);
        }

        public ListWebElement GetAll()
        {
            return this.Parent != null ? new ListWebElement(this.Locator, this.Parent) : new ListWebElement(this.Locator, this.WrappedDriver);
        }
        public ListWebElement GetAll(string xpath)
        {
            return GetAll(By.XPath(xpath));
        }
        public ListWebElement GetAll(By locator)
        {
            return new ListWebElement(locator, this);
        }
        public ListWebElement GetAllChildren()
        {
            return GetAll(By.XPath(".//*"));
        }

        public WebElement Locate()
        {
            return this.Parent != null ? new WebElement(this.Element, this.Parent) : new WebElement(this.Element, this.WrappedDriver);
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
            this.WrappedDriver.ExecuteJavaScript($"window.scrollTo({elem.Location.X}, {elem.Location.Y})", elem);
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

        private IElementLocator GetElementLocator()
        {
            var context = this.Parent?.WrappedElement ?? (ISearchContext)this.WrappedDriver;
            return new DefaultElementLocator(context);
        }
    }
}
