namespace WebDriverFramework.Extension
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.Extensions;

    public static partial class WebElementExtension
    {
        public static void ExecuteJavaScript(this IWebElement element, string script, params object[] args)
        {
            element.GetDriver().ExecuteJavaScript(script, args);
        }
        public static T ExecuteJavaScript<T>(this IWebElement element, string script, params object[] args)
        {
            return element.GetDriver().ExecuteJavaScript<T>(script, args);
        }

        public static string GetJsText(this IWebElement element)
        {
            return element.GetDriver().ExecuteJavaScript<string>("arguments[0].text", element);
        }
        public static T JSClick<T>(this T element) where T : IWebElement
        {
            element.GetDriver().ExecuteJavaScript("arguments[0].click()", element);
            return element;
        }
        public static T JSScrollIntoView<T>(this T element) where T : IWebElement
        {
            element.GetDriver().ExecuteJavaScript("arguments[0].scrollIntoView(true)", element);
            return element;
        }
        public static T JSScrollTo<T>(this T element) where T : IWebElement
        {
            element.GetDriver().ExecuteJavaScript($"window.scrollTo({element.Location.X}, {element.Location.Y})", element);
            return element;
        }
    }
}
