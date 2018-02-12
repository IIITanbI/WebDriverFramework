namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using Proxy;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class ListWebElement : IElementList
    {
        private IList<IWebElement> _proxiedElements;

        public ListWebElement(By locator, IWebDriver driver) : this(locator, null, driver)
        {
        }
        public ListWebElement(By locator, WebElement parent, IWebDriver driver)
        {
            this.Locator = locator;
            this.Parent = parent;
            this.WrappedDriver = driver;
            this._proxiedElements = GetProxy(locator, parent, driver, false);
        }

        public List<WebElement> Elements => this._proxiedElements.Select(CreateElement).ToList();
        public int Count => this.Elements.Count;
        public WebElement this[int index] => this.Elements[index];

        public IWebDriver WrappedDriver { get; }
        public By Locator { get; }
        public WebElement Parent { get; }

        public List<WebElement> Get(By locator)
        {
            return this.Elements.Select(e => e.Get(locator)).ToList();
        }
        public WebElement GetByText(string text)
        {
            return this.Elements.FirstOrDefault(e => e.Text.Trim() == text);
        }

        public IElementList Locate()
        {
            this._proxiedElements = this.Elements.Select(e => e.Locate().Element).ToList();
            return this;
        }
        public IElementList CheckStaleness()
        {
            this.Elements.ForEach(e => e.CheckStaleness());
            return this;
        }

        public IEnumerator<WebElement> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Elements).GetEnumerator();
        }

        private WebElement CreateElement(IWebElement element)
        {
            return new WebElement(element, this.WrappedDriver);
        }
        private static IList<IWebElement> GetProxy(By locator, WebElement parent, IWebDriver driver, bool cache)
        {
            return (IList<IWebElement>)new WebElementListProxy(typeof(IList<IWebElement>), new DefaultElementLocator((ISearchContext)parent?.WrappedElement ?? driver),
                new[] { locator }, cache).GetTransparentProxy();
        }
    }
}