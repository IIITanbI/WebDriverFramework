namespace WebDriverFramework.Extension
{
    using System;
    using OpenQA.Selenium.Support.UI;

    public static class WebDriverWaitExtension
    {
        public static T Until<T, TInput>(this IWait<TInput> wait, Func<T> condition)
        {
            return wait.Until((TInput input) => condition());
        }
    }
}