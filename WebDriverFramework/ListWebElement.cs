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
        private WebElementListProxy _webElementsProxy;

        public ListWebElement(By locator, IWebDriver driver) : this(locator, null, driver)
        {
        }
        public ListWebElement(By locator, WebElement parent) : this(locator, parent, parent.WrappedDriver)
        {
        }
        private ListWebElement(By locator, WebElement parent, IWebDriver driver)
        {
            this.Locator = locator;
            this.Parent = parent;
            this.WrappedDriver = driver;
            this._webElementsProxy = new WebElementListProxy(typeof(IList<IWebElement>), new DefaultElementLocator(parent?.WrappedElement ?? (ISearchContext)driver), new[] { locator }, false);
        }
        private ListWebElement(List<IWebElement> elements)
        {
            this._webElementsProxy = new WebElementListProxy(elements);
        }

        public List<WebElement> Elements => this._webElementsProxy.Elements.Select(CreateElement).ToList();
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
            this.Elements.ForEach(e => e.StubActionOnElement());
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
    }
}