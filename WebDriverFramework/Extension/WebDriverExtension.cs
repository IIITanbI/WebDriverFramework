namespace WebDriverFramework.Extension
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using System;
    using System.Collections.Generic;

    public class WebDriverImplicitWaitHelper
    {
        private static readonly Dictionary<IWebDriver, WebDriverImplicitWaitHelper> cachedDrivers = new Dictionary<IWebDriver, WebDriverImplicitWaitHelper>();
        public static WebDriverImplicitWaitHelper RegisterDriver(IWebDriver driver, double implicitWait)
        {
            if (cachedDrivers.TryGetValue(driver, out var value))
            {
                return value;
            }

            var waiter = new WebDriverImplicitWaitHelper(driver, implicitWait);
            cachedDrivers.Add(driver, waiter);
            return waiter;
        }
        public static WebDriverImplicitWaitHelper GetHelper(IWebDriver driver)
        {
            return cachedDrivers[driver];
        }



        private IWebDriver driver;
        private Stack<double> timeouts = new Stack<double>();

        private WebDriverImplicitWaitHelper(IWebDriver driver, double implicitWait)
        {
            this.driver = driver;
            this.InitialImplicitWait = implicitWait;
            SetImplicitWait(implicitWait);
        }

        public double ImplicitWait
        {
            get => timeouts.Peek();
            set => SetImplicitWait(value);
        }
        public double InitialImplicitWait { get; }

        public void SetImplicitWait(double seconds)
        {
            timeouts.Push(seconds);
            _setImplicitWait(seconds);
        }
        public bool RevertImplicitWait()
        {
            var revertSuccess = false;
            if (timeouts.Count > 1)
            {
                timeouts.Pop();
                revertSuccess = true;
            }

            return revertSuccess;
        }

        public void DoWithImplicitWait(Action action, double implicitWait)
        {
            driver.DoWithImplicitWait<object>(() =>
            {
                action();
                return null;
            }, implicitWait);
        }
        public T DoWithImplicitWait<T>(Func<T> func, double implicitWait)
        {
            implicitWait = implicitWait >= 0 ? implicitWait : this.ImplicitWait;

            try
            {
                driver.SetImplicitWait(implicitWait);
                return func();
            }
            finally
            {
                driver.RevertImplicitWait();
            }
        }

        private void _setImplicitWait(double seconds)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(seconds);
        }
    }

    public static class WebDriverImplicitWaitExtension
    {
        public static WebDriverImplicitWaitHelper RegisterDriver(this IWebDriver driver, double implicitWait) => WebDriverImplicitWaitHelper.RegisterDriver(driver, implicitWait);

        public static double GetImplicitWait(this IWebDriver driver) => _getHelper(driver).ImplicitWait;
        public static void SetImplicitWait(this IWebDriver driver, double seconds) => _getHelper(driver).SetImplicitWait(seconds);
        public static bool RevertImplicitWait(this IWebDriver driver) => _getHelper(driver).RevertImplicitWait();

        public static void DoWithImplicitWait(this IWebDriver driver, Action action, double implicitWait) => _getHelper(driver).DoWithImplicitWait(action, implicitWait);
        public static T DoWithImplicitWait<T>(this IWebDriver driver, Func<T> func, double implicitWait) => _getHelper(driver).DoWithImplicitWait(func, implicitWait);

        private static WebDriverImplicitWaitHelper _getHelper(this IWebDriver driver) => WebDriverImplicitWaitHelper.GetHelper(driver);
    }

    public static class WebDriverExtension
    {
        public static WebDriverWait GetWait(this IWebDriver driver, double timeout, params Type[] exceptionTypes)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.IgnoreExceptionTypes(exceptionTypes);
            return wait;
        }

        public static T Wait<T>(this IWebDriver driver, Func<IWebDriver, T> condition, double timeout, params Type[] exceptionTypes)
        {
            return driver.GetWait(timeout, exceptionTypes).Until(condition);
        }
        public static T Wait<T>(this IWebDriver driver, Func<T> condition, double timeout, params Type[] exceptionTypes)
        {
            return driver.GetWait(timeout, exceptionTypes).Until(condition);
        }
    }
}