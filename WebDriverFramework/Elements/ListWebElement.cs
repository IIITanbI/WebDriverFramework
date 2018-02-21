using OpenQA.Selenium.Support.PageObjects;
using WebDriverFramework.Extension;

namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using Proxy;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class ListWebElement : IReadOnlyList<WebElement>
    {
        private WebElement _parent;

        public ListWebElement(List<IWebElement> elements, IWebDriver driver) : this(new WebElementListProxy(elements), null, driver)
        {
        }
        public ListWebElement(By locator, IWebDriver driver) : this(new[] { locator }, driver)
        {
        }
        public ListWebElement(IEnumerable<By> locators, IWebDriver driver) : this(null, null, driver)
        {
            this.WebElementsProxy = new WebElementListProxy(locators, driver);
        }
        public ListWebElement(By locator, WebElement parent) : this(new WebElementListProxy(new[] { locator }, parent.WrappedElement), parent, parent.WrappedDriver)
        {
        }

        private ListWebElement(WebElementListProxy webElementsProxy, WebElement parent, IWebDriver driver)
        {
            this.WebElementsProxy = webElementsProxy;
            this.Parent = parent;
            this.WrappedDriver = driver;
        }

        protected WebElementListProxy WebElementsProxy { get; }
        public WebElement Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                this.WebElementsProxy.Locator = this.GetParentElementLocator();
            }
        }
        public IWebDriver WrappedDriver { get; }
        public List<WebElement> Elements => this.WebElementsProxy.Elements.Select(e => new WebElement(e, this.WrappedDriver)).ToList();
        public List<By> Locators => this.WebElementsProxy.Bys.ToList();
        public By Locator => Locators.First();

        public int Count => this.Elements.Count;
        public WebElement this[int index] => this.Elements[index];

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
            return new ListWebElement(elements, this.WrappedDriver);
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

        protected IElementLocator GetParentElementLocator()
        {
            return new DefaultElementLocator(this.Parent != null ? this.Parent.WrappedElement : (ISearchContext)this.WrappedDriver);
        }
    }
}