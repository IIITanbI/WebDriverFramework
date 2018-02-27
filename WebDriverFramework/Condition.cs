namespace WebDriverFramework
{
    public static class Condition
    {
        public static bool Exist(BaseWebElement e) => e.Exist;
        public static bool NotExist(BaseWebElement e) => !e.Exist;
        public static bool Displayed(BaseWebElement e) => e.Exist && e.Displayed;
        public static bool NotDisplayed(BaseWebElement e) => !e.Displayed;
    }
}