namespace WebDriverFramework
{
    using OpenQA.Selenium;

    public interface IElement
    {
        IWebElement Element { get; }
    }
}