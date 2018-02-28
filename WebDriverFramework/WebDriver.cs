namespace WebDriverFramework
{
    using Elements;
    using Extension;
    using OpenQA.Selenium;
    using System.Collections.Generic;
    using System.Linq;

    public class WebDriver
    {
        public WebDriver(IWebDriver driver)
        {
            this.WrappedDriver = driver;
        }

        public IWebDriver WrappedDriver { get; }

        public T Find<T>(string xpath) where T : ILocate<T> => Find<T>(By.XPath(xpath));
        public T Find<T>(By locator) where T : ILocate<T> => ElementFactory.Create<T>(locator, null, this.WrappedDriver).Locate();
        public IList<T> FindAll<T>(string xpath = ".//*") where T : ILocate<T> => FindAll<T>(By.XPath(xpath));
        public IList<T> FindAll<T>(By locator) where T : ILocate<T> => GetAll<T>(locator).LocateAll().ToList();

        public T Get<T>(string xpath) => Get<T>(By.XPath(xpath));
        public T Get<T>(By locator) => ElementFactory.Create<T>(locator, null, this.WrappedDriver);
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

        public void Quit()
        {
            this.WrappedDriver.Quit();
        }
    }
}
