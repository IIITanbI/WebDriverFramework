using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using WebDriverFramework.Extension;
using WebDriverFramework.Proxy;

namespace WebDriverFramework
{
    public static class WebElementExtension
    {
        public static TElement Click<TElement>(this TElement element) where TElement : WebElement
        {
            element.Element.Click();
            return element;
        }

        public static TResult Wait<TElement, TResult>(this TElement element, Func<TElement, TResult> condition, double timeout = -1, params Type[] exceptionTypes) where TElement : WebElement
        {
            timeout = timeout < 0 ? WebElement.DefaultWaitTimeout : timeout;
            return element.WrappedDriver.Wait<TResult>(d => condition(element), timeout, exceptionTypes);
        }
        public static TElement WaitUntil<TElement>(this TElement element, Func<TElement, bool> condition, double timeout = -1, params Type[] exceptionTypes) where TElement : WebElement
        {
            element.Wait(condition, timeout, exceptionTypes);
            return element;
        }
        public static bool TryWait(this WebElement element, Func<WebElement, bool> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            try
            {
                return Wait(element, condition, timeout, exceptionTypes);
            }
            catch
            {
                return false;
            }
        }
        public static bool ShouldBe(this WebElement element, Func<WebElement, bool> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            return TryWait(element, condition, timeout, exceptionTypes);
        }
        public static bool ShouldNot(this WebElement element, Func<WebElement, bool> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            return TryWait(element, e => !condition(e), timeout, exceptionTypes);
        }
    }

    public static class ElementFactory
    {
        public static object Create(Type type, By locator, WebElement parent, IWebDriver driver = null)
        {
            return Activator.CreateInstance(type, new object[] { locator, parent, driver });
        }
        public static object Create(Type type, IWebElement implicitElement, IWebDriver driver)
        {
            return Activator.CreateInstance(type, new object[] { implicitElement, driver });
        }

        public static TElement Create<TElement>(By locator, WebElement parent, IWebDriver driver = null)
        {
            return (TElement)Activator.CreateInstance(typeof(TElement), new object[] { locator, parent, driver });
        }
        public static TElement Create<TElement>(IWebElement implicitElement, IWebDriver driver)
        {
            return (TElement)Activator.CreateInstance(typeof(TElement), new object[] { implicitElement, driver });
        }
    }

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

    public abstract class WebElement : IBaseWebElement, IFindElement, IFindElement<LabelElement>
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
        protected WebElement(string xpath, WebElement parent, IWebDriver driver = null) : this(By.XPath(xpath), parent, driver)
        {
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

        public WebElementProxy WebElementProxy { get; }
        public IWebDriver WrappedDriver { get; }
        public IWebElement ProxyElement => (IWebElement)this.WebElementProxy.GetTransparentProxy();
        public IWebElement Element => this.ProxyElement.Unwrap();

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
        public bool Exist => this.Element.Exist();

        public bool Displayed => Element.Displayed;

        public string TagName => Element.TagName;
        public Point Location => Element.Location;
        public Size Size => Element.Size;

        public string GetAttribute(string attributeName) => Element.GetAttribute(attributeName);
        public string GetProperty(string propertyName) => Element.GetProperty(propertyName);
        public string GetCssValue(string propertyName) => Element.GetCssValue(propertyName);
        #endregion

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

        protected IElementLocator GetParentElementLocator()
        {
            return new DefaultElementLocator(this.Parent != null ? this.Parent.ProxyElement : (ISearchContext)this.WrappedDriver);
        }
    }

    public class LabelElement : WebElement, ILocate<LabelElement>
    {
        public LabelElement(IWebElement implicitElement, IWebDriver driver) : base(implicitElement, driver)
        {
        }
        public LabelElement(By locator, WebElement parent, IWebDriver driver = null) : base(locator, parent, driver)
        {
        }

        public string Text => Element.Text;

        public LabelElement Locate() => new LabelElement(this.Element, this.WrappedDriver);
    }

    public abstract class InputWebElement : WebElement
    {
        protected InputWebElement(IWebElement implicitElement, IWebDriver driver) : base(implicitElement, driver)
        {
        }
        protected InputWebElement(By locator, WebElement parent, IWebDriver driver = null) : base(locator, parent, driver)
        {
        }

        public string Value => GetAttribute("value");
    }

    public class TextWebElement : InputWebElement, ILocate<TextWebElement>
    {
        public TextWebElement(IWebElement implicitElement, IWebDriver driver) : base(implicitElement, driver)
        {
        }
        public TextWebElement(By locator, WebElement parent, IWebDriver driver = null) : base(locator, parent, driver)
        {
        }

        public bool Enabled => Element.Enabled;

        public TextWebElement Clear()
        {
            this.Element.Clear();
            return this;
        }
        public TextWebElement SendKeys(string text)
        {
            this.Element.SendKeys(text);
            return this;
        }
        public TextWebElement Submit()
        {
            this.Element.Submit();
            return this;
        }

        public TextWebElement Locate() => new TextWebElement(this.Element, this.WrappedDriver);
    }

    public class CheckBox : InputWebElement, ILocate<CheckBox>
    {
        public CheckBox(IWebElement implicitElement, IWebDriver driver) : base(implicitElement, driver)
        {
        }
        public CheckBox(By locator, WebElement parent, IWebDriver driver = null) : base(locator, parent, driver)
        {
        }

        public bool Selected => Element.Selected;
        public CheckBox Locate() => new CheckBox(this.Element, this.WrappedDriver);
    }
}