namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using System.Collections.ObjectModel;

    public class WebDriverSearchContext : ISearchContext
    {
        public WebDriver Driver { get; }

        public WebDriverSearchContext(WebDriver driver)
        {
            this.Driver = driver;
        }

        public IWebElement FindElement(By by) => this.Driver.NativeDriver.FindElement(by);
        public ReadOnlyCollection<IWebElement> FindElements(By by) => this.Driver.NativeDriver.FindElements(by);
    }
}