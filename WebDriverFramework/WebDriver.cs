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

        public WebElement Get(string xpath)
        {
            return Get(By.XPath(xpath));
        }
        public WebElement Get(By locator)
        {
            return new WebElement(locator, this.WrappedDriver);
        }
        public ListWebElement GetAll(By locator)
        {
            return new ListWebElement(locator, this.WrappedDriver);
        }
        public ListWebElement GetAll(string xpath)
        {
            return GetAll(By.XPath(xpath));
        }

        public WebElement WaitForPresent(By locator, double timeout = -1)
        {
            return this.Get(locator).WaitUntil(Condition.Exist, timeout);
        }
        public WebElement WaitForPresent(string xpath, double timeout = -1)
        {
            return WaitForPresent(By.XPath(xpath), timeout);
        }

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

        public IWebElement FindElement(By by)
        {
            return this.WrappedDriver.FindElement(by);
        }
        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return this.WrappedDriver.FindElements(by);
        }
        public void Dispose()
        {
            WrappedDriver.Dispose();
        }
        #endregion
    }
}
