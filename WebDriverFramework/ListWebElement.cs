namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using Proxy;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class ListWebElement : IReadOnlyList<WebElement>
    {
        private WebElementListProxy _webElementsProxy;
        private WebElementListProxy WebElementsProxy
        {
            get => _webElementsProxy;
            set
            {
                this._webElementsProxy = value;
                this._webElementsProxy.Source = this;
            }
        }

        public ListWebElement(List<IWebElement> elements, IWebDriver driver) : this(new WebElementListProxy(elements), driver)
        {
        }
        public ListWebElement(List<IWebElement> elements, WebElement parent) : this(new WebElementListProxy(elements), parent)
        {
        }

        public ListWebElement(By locator, IWebDriver driver) : this(null, null, driver)
        {
            this.WebElementsProxy = new WebElementListProxy(locator, this);
        }
        public ListWebElement(By locator, WebElement parent) : this(null, parent, parent.WrappedDriver)
        {
            this.WebElementsProxy = new WebElementListProxy(locator, this);
        }

        public ListWebElement(WebElementListProxy webElementsProxy, IWebDriver driver) : this(webElementsProxy, null, driver)
        {
        }
        public ListWebElement(WebElementListProxy webElementsProxy, WebElement parent) : this(webElementsProxy, parent, parent.WrappedDriver)
        {
        }

        private ListWebElement(WebElementListProxy webElementsProxy, WebElement parent, IWebDriver driver)
        {
            this.WebElementsProxy = webElementsProxy;
            this.Parent = parent;
            this.WrappedDriver = driver;
        }

        public List<WebElement> Elements => this.WebElementsProxy.Elements.Select(CreateElement).ToList();
        public int Count => this.Elements.Count;
        public WebElement this[int index] => this.Elements[index];

        public IWebDriver WrappedDriver { get; }
        public List<By> Locators => this.WebElementsProxy.Bys.ToList();
        public By Locator => Locators.First();
        public WebElement Parent { get; set; }

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
            var elements = this.Elements.Select(e => e.Locate().Element).ToList();
            return this.Parent != null ? new ListWebElement(elements, this.Parent) : new ListWebElement(elements, this.WrappedDriver);
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