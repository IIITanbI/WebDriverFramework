using System;
using System.Collections.Generic;

namespace WebDriverFramework
{
    public class Condition
    {
        public static bool Exist(By locator) => e.Exist;
        public static bool Exist(WebElement e) => e.Exist;
        public static bool NotExist(WebElement e) => !Exist(e);
        public static bool Displayed(WebElement e) => e.Exist && e.Displayed;
        public static bool NotDisplayed(WebElement e) => !Displayed(e);
    }

    public class MyCondition
    {
        protected Dictionary<WebElement, int> callDictionary = new Dictionary<WebElement, int>(3);
        protected Func<WebElement, bool> func;

        public static implicit operator Func<WebElement, bool>(MyCondition mc)
        {
            return mc.func;
        }
    }
}