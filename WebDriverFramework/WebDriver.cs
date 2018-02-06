using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Support.UI;

namespace WebDriverFramework
{
    using OpenQA.Selenium;

    public class WebDriver : IWebDriver
    {
        internal IWebDriver Driver { get; }

        public WebDriver(IWebDriver driver)
        {
            this.Driver = driver;
        }


        public WebDriverWait GetWait(TimeSpan timeout, params Type[] exceptionTypes)
        {
            var wait = new WebDriverWait(this.Driver, timeout);
            wait.IgnoreExceptionTypes(exceptionTypes);
            return wait;
        }

        public IWebElement WaitForElement(TimeSpan timeout, By b)
        {
            return WaitT(d => d.FindElement(b), timeout);
        }

        public IWebElement WaitForElement(TimeSpan timeout, IWebElement elem)
        {
            // use any object's method or property (field) to unwrap it from proxy
            WaitT(d => elem.GetType(), timeout, typeof(NoSuchElementException));
            return elem;
        }

        public T WaitT<T>(Func<IWebDriver, T> condition, TimeSpan timeout, params Type[] exceptionTypes)
        {
            return GetWait(timeout, exceptionTypes).Until(condition);
        }


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

        public string Url
        {
            get => Driver.Url;
            set => Driver.Url = value;
        }

        public string Title => this.Driver.Title;
        public string PageSource => Driver.PageSource;

        public string CurrentWindowHandle => Driver.CurrentWindowHandle;

        public ReadOnlyCollection<string> WindowHandles => Driver.WindowHandles;

        public IWebElement FindElement(By by)
        {
            return Driver.FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return Driver.FindElements(by);
        }

        public void Dispose()
        {
            Driver.Dispose();
        }
    }
}
