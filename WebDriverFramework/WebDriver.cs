namespace WebDriverFramework
{
    using Extension;
    using OpenQA.Selenium;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class WebDriver : IWebDriver
    {
        public double ImplicitWait => WebDriverImplicitWaitHelper.GetHelper(this).ImplicitWait;
        public double InitialImplicitWait => WebDriverImplicitWaitHelper.GetHelper(this).InitialImplicitWait;

        public WebDriver(IWebDriver driver) : this(driver, 0.5)
        {
        }
        public WebDriver(IWebDriver driver, double implicitWait)
        {
            this.Driver = driver;
            WebDriverImplicitWaitHelper.RegisterDriver(this, implicitWait);
        }

        public IWebDriver Driver { get; }

        public WebElement Get(string xpath)
        {
            return Get(By.XPath(xpath));
        }
        public WebElement Get(By locator)
        {
            return new WebElement(locator, this.Driver);
        }
        private WebElement Get(IWebElement implicitElement)
        {
            return new WebElement(implicitElement, this.Driver);
        }

        public WebElement Locate(By locator)
        {
            return new WebElement(locator, this.Driver).Locate();
        }
        public WebElement Locate(By locator, double timeout, double implicitWait = -1)
        {
            return WaitForElement(locator, timeout, true, implicitWait);
        }

        public WebElement WaitForElement(By locator, double timeout, double implicitWait = -1)
        {
            return WaitForElement(locator, timeout, false, implicitWait);
        }
        public WebElement WaitForElement(By locator, double timeout, bool locate, double implicitWait = -1)
        {
            var element = this.Get(locator);
            this.Driver.DoWithImplicitWait(() => element.WaitForPresent(timeout), implicitWait);
            return locate ? element.Locate() : element;
        }
        #region MyRegion
        public string Url
        {
            get => Driver.Url;
            set => Driver.Url = value;
        }
        public string Title => this.Driver.Title;
        public string PageSource => Driver.PageSource;
        public string CurrentWindowHandle => Driver.CurrentWindowHandle;
        public ReadOnlyCollection<string> WindowHandles => Driver.WindowHandles;

        public void Close()
        {
            Driver.Close();
        }
        public void Quit()
        {
            Driver.Quit();
        }
        public IOptions Manage()
        {
            return Driver.Manage();
        }
        public INavigation Navigate()
        {
            return Driver.Navigate();
        }
        public ITargetLocator SwitchTo()
        {
            return Driver.SwitchTo();
        }

        public IWebElement FindElement(By by)
        {
            return this.Get(by).Locate();
        }
        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return this.Driver.FindElements(by).Select(this.Get).Cast<IWebElement>().ToList().AsReadOnly();
        }
        public void Dispose()
        {
            Driver.Dispose();
        }
        #endregion
    }
}
