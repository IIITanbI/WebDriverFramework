namespace WebDriverFramework
{
    using OpenQA.Selenium;

    public interface IGetElement
    {
        T Get<T>(By locator);
    }
}