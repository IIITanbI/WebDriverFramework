using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebDriverFramework.Extension;

namespace WebDriverFramework
{
    public class Condition
    {
        public static bool Exist(IWebElement e) => e.Exist();
        public static bool NotExist(IWebElement e) => !Exist(e);
        public static bool Displayed(IWebElement e) => e.Exist() && e.Displayed;
        public static bool NotDisplayed(IWebElement e) => !Displayed(e);
    }
}