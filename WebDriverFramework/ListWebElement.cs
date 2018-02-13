namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using Proxy;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class ListWebElement : IReadOnlyList<WebElement>
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
            this._proxiedElements = GetProxy(locator, (ISearchContext)parent?.WrappedElement ?? driver, false);
        }
        private ListWebElement(IList<IWebElement> proxiedElements)
        {
            this._proxiedElements = proxiedElements;
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

        public ListWebElement Locate()
        {
            return new ListWebElement(this.Elements.Select(e => e.Locate().Element).ToList());
        }
        public ListWebElement CheckStaleness()
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
        private static IList<IWebElement> GetProxy(By locator, ISearchContext context, bool cache)
        {
            return (IList<IWebElement>)new WebElementListProxy(typeof(IList<IWebElement>), new DefaultElementLocator(context), new[] { locator }, cache).GetTransparentProxy();
        }
    }
}