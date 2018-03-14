namespace WebDriverFramework
{
    using Elements;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.Extensions;
    using OpenQA.Selenium.Support.PageObjects;
    using OpenQA.Selenium.Support.UI;
    using Proxy;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class WebDriver : IGetElement, IGetElements
    {
        public WebDriver(IWebDriver nativeDriver)
        {
            this.NativeDriver = nativeDriver;
        }

        public IWebDriver NativeDriver { get; }

        public T Get<T>(By locator) => ElementFactory.Create<T>(locator, null, this);
        public IEnumerable<T> GetAll<T>(By locator)
        {
            return new WebElementListProxy(new[] { locator }, new DefaultElementLocator(this.NativeDriver), false)
                .Elements.Select(e => ElementFactory.Create<T>(e, this));
        }

        public WebDriverWait GetWait(double timeout, params Type[] exceptionTypes)
        {
            var wait = new WebDriverWait(this.NativeDriver, TimeSpan.FromSeconds(timeout));
            wait.IgnoreExceptionTypes(exceptionTypes);
            return wait;
        }
        public T Wait<T>(Func<WebDriver, T> condition, double timeout, params Type[] exceptionTypes)
        {
            return this.GetWait(timeout, exceptionTypes).Until(d => condition(this));
        }

        public void ExecuteJavaScript(string script, WebElement element, ILogger log = null)
        {
            this.ExecuteJavaScript<object>(script, element, log);
        }
        public T ExecuteJavaScript<T>(string script, WebElement element, ILogger log = null)
        {
            return this.ExecuteJavaScript<T>(script, new object[] { element.Element }, log);
        }

        public void ExecuteJavaScript(string script, object[] args, ILogger log = null)
        {
            this.ExecuteJavaScript<object>(script, args, log);
        }
        public T ExecuteJavaScript<T>(string script, object[] args, ILogger log = null)
        {
            return this.NativeDriver.ExecuteJavaScript<T>(script, args);
        }

        public void Quit()
        {
            this.NativeDriver.Quit();
        }
    }
}
