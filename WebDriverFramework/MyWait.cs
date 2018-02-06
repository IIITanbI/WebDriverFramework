using System.Collections.ObjectModel;

namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using System;

    public class MyWait : WebDriverWait
    {
        private class WebDriverStub : IWebDriver
        {
            public IWebElement FindElement(By @by)
            {
                throw new NotImplementedException();
            }

            public ReadOnlyCollection<IWebElement> FindElements(By @by)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void Close()
            {
                throw new NotImplementedException();
            }

            public void Quit()
            {
                throw new NotImplementedException();
            }

            public IOptions Manage()
            {
                throw new NotImplementedException();
            }

            public INavigation Navigate()
            {
                throw new NotImplementedException();
            }

            public ITargetLocator SwitchTo()
            {
                throw new NotImplementedException();
            }

            public string Url { get; set; }
            public string Title { get; }
            public string PageSource { get; }
            public string CurrentWindowHandle { get; }
            public ReadOnlyCollection<string> WindowHandles { get; }
        }

        private static TimeSpan DefaultSleepTimeout => TimeSpan.FromMilliseconds(500);

        public MyWait(TimeSpan timeout) : this(timeout, DefaultSleepTimeout)
        {
        }

        public MyWait(TimeSpan timeout, TimeSpan sleepInterval) : this(new WebDriverStub(), timeout, sleepInterval)
        {
        }

        public MyWait(IWebDriver driver, TimeSpan timeout) : this(driver, timeout, DefaultSleepTimeout)
        {
        }

        public MyWait(IWebDriver driver, TimeSpan timeout, TimeSpan sleepInterval) : base(new SystemClock(), driver, timeout, sleepInterval)
        {
        }

        //Use wait without IWebDriver
        public TResult Until<TResult>(Func<TResult> condition)
        {
            return Until(driver => condition());
        }
    }
}