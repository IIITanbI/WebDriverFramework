namespace WebDriverFramework
{
    using Extension;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Internal;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class WebDriver : IWebDriver, IWrapsDriver
    {
        public double ImplicitWait => WebDriverImplicitWaitHelper.GetHelper(this).ImplicitWait;
        public double InitialImplicitWait => WebDriverImplicitWaitHelper.GetHelper(this).InitialImplicitWait;

        public WebDriver(IWebDriver driver, double implicitWait = 0)
        {
            this.WrappedDriver = driver;
            WebDriverImplicitWaitHelper.RegisterDriver(this, implicitWait);
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
        private WebElement Get(IWebElement implicitElement)
        {
            return new WebElement(implicitElement, this.WrappedDriver);
        }

        public WebElement WaitForElement(By locator, double timeout = -1)
        {
            return this.Get(locator).WaitForPresent(timeout);
        }
        public WebElement WaitForElement(string xpath, double timeout = -1)
        {
            return WaitForElement(By.XPath(xpath), timeout);
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
            return this.Get(by).Locate();
        }
        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return this.WrappedDriver.FindElements(by).Select(this.Get).Cast<IWebElement>().ToList().AsReadOnly();
        }
        public void Dispose()
        {
            WrappedDriver.Dispose();
        }
        #endregion
    }
}
