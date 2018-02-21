namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using Extension;

    public static class Condition
    {
        public static bool Exist(IWebElement e) => e.Exist();
        public static bool NotExist(IWebElement e) => !Exist(e);
        public static bool Displayed(IWebElement e) => e.Exist() && e.Displayed;
        public static bool NotDisplayed(IWebElement e) => !Displayed(e);
    }
}