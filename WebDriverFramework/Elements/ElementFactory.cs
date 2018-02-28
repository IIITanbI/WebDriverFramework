using System;
using OpenQA.Selenium;

namespace WebDriverFramework.Elements
{
    public static class ElementFactory
    {
        public static object Create(Type type, By locator, WebElement parent, IWebDriver driver = null)
        {
            return Activator.CreateInstance(type, new object[] { locator, parent, driver });
        }
        public static object Create(Type type, IWebElement implicitElement, IWebDriver driver)
        {
            return Activator.CreateInstance(type, new object[] { implicitElement, driver });
        }

        public static TElement Create<TElement>(By locator, WebElement parent, IWebDriver driver = null)
        {
            return (TElement)Activator.CreateInstance(typeof(TElement), new object[] { locator, parent, driver });
        }
        public static TElement Create<TElement>(IWebElement implicitElement, IWebDriver driver)
        {
            return (TElement)Activator.CreateInstance(typeof(TElement), new object[] { implicitElement, driver });
        }
    }
}