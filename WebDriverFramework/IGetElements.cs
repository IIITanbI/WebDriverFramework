namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using System.Collections.Generic;

    public interface IGetElements
    {
        IEnumerable<T> GetAll<T>(By locator);
    }
}