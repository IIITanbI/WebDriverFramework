using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using WebDriverFramework.Elements;

namespace WebDriverFramework
{
    class FindAllHelper<T> : IEnumerable<T>
    {
        private readonly WebElement _element;
        private readonly WebDriver _driver;
        private readonly By _locator;

        public FindAllHelper(WebElement element, By locator)
        {
            this._element = element;
            this._locator = locator;
        }
        public FindAllHelper(WebDriver driver, By locator)
        {
            this._driver = driver;
            this._locator = locator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var context = this._element?.Element ?? (ISearchContext)this._driver.NativeDriver;
            return context.FindElements(this._locator)
                .Select(e => ElementFactory.Create<T>(e, this._element.Driver))
                .GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}