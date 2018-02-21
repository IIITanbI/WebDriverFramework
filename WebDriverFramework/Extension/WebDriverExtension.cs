namespace WebDriverFramework.Extension
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using System;

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
    }
}