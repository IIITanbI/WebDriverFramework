namespace WebDriverFramework
{
    using Elements;

    public static class Condition
    {
        public static bool Exist(WebElement e) => e.Exist;
        public static bool NotExist(WebElement e) => !e.Exist;
        public static bool Displayed(WebElement e) => e.Exist && e.Displayed;
        public static bool NotDisplayed(WebElement e) => !e.Exist || !e.Displayed;
    }
}