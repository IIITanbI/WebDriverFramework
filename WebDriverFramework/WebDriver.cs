using System.Collections.Generic;
using System.Linq;

namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Internal;
    using System.Collections.ObjectModel;

    public class WebDriver : IWebDriver, IWrapsDriver
    {
        public WebDriver(IWebDriver driver)
        {
            this.WrappedDriver = driver;
        }

        public IWebDriver WrappedDriver { get; }

        public T Find<T>(string xpath) where T : ILocate<T> => Find<T>(By.XPath(xpath));
        public T Find<T>(By locator) where T : ILocate<T> => ElementFactory.Create<T>(locator, null, this).Locate();
        public IList<T> FindAll<T>(string xpath = ".//*") where T : ILocate<T> => FindAll<T>(By.XPath(xpath));
        public IList<T> FindAll<T>(By locator) where T : ILocate<T> => GetAll<T>(locator).LocateAll().ToList();

        public T Get<T>(string xpath) => Get<T>(By.XPath(xpath));
        public T Get<T>(By locator) => ElementFactory.Create<T>(locator, null, this);
        public IEnumerable<T> GetAll<T>(string xpath) => GetAll<T>(By.XPath(xpath));
        public IEnumerable<T> GetAll<T>(By locator) => this.WrappedDriver.FindElements(locator).Select(e => ElementFactory.Create<T>(e, this));

        public LabelElement Find(string xpath) => Find<LabelElement>(xpath);
        public LabelElement Find(By locator) => Find<LabelElement>(locator);
        public IList<LabelElement> FindAll(string xpath = ".//*") => FindAll<LabelElement>(xpath);
        public IList<LabelElement> FindAll(By locator) => FindAll<LabelElement>(locator);

        public LabelElement Get(string xpath) => Get<LabelElement>(xpath);
        public LabelElement Get(By locator) => Get<LabelElement>(locator);
        public IEnumerable<LabelElement> GetAll(string xpath) => GetAll<LabelElement>(xpath);
        public IEnumerable<LabelElement> GetAll(By locator) => GetAll<LabelElement>(locator);

        public WebElement WaitForPresent(By locator, double timeout = -1) => this.Get(locator).WaitUntil(Condition.Exist, timeout);
        public WebElement WaitForPresent(string xpath, double timeout = -1) => WaitForPresent(By.XPath(xpath), timeout);

        #region MyRegion
        public string Url
        {
            get => WrappedDriver.Url;
            set => WrappedDriver.Url = value;
        }
        public string Title => this.WrappedDriver.Title;
        public string PageSource => WrappedDriver.PageSource;
        public string CurrentWindowHandle => WrappedDriver.CurrentWindowHandle;
        public ReadOnlyCollection<string> WindowHandles => WrappedDriver.WindowHandles;

        public void Close()
        {
            WrappedDriver.Close();
        }
        public void Quit()
        {
            WrappedDriver.Quit();
        }
        public IOptions Manage()
        {
            return WrappedDriver.Manage();
        }
        public INavigation Navigate()
        {
            return WrappedDriver.Navigate();
        }
        public ITargetLocator SwitchTo()
        {
            return WrappedDriver.SwitchTo();
        }

        public WebElement FindElement(By by)
        {
            return null;
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return this.WrappedDriver.FindElements(by);
        }
        public void Dispose()
        {
            WrappedDriver.Dispose();
        }

        IWebElement ISearchContext.FindElement(By by)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
