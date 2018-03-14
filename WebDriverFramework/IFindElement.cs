namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using System.Collections.Generic;

    public interface IFindElement
    {
        T Get<T>(By locator);
        IEnumerable<T> GetAll<T>(By locator);
    }
}