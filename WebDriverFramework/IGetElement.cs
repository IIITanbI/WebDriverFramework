namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using System.Collections.Generic;

    public interface IGetElement
    {
        T Get<T>(By locator);
        //IEnumerable<T> GetAll<T>(By locator);
    }

    public interface IGetElements
    {
        IEnumerable<T> GetAll<T>(By locator);
    }
}