namespace WebDriverFramework.Extension
{
    using Elements;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class WebElementExtension
    {
        public static TResult Wait<TElement, TResult>(this TElement element, Func<TElement, TResult> condition, double timeout = -1, params Type[] exceptionTypes) where TElement : WebElement
        {
            timeout = timeout < 0 ? WebElement.DefaultWaitTimeout : timeout;
            return element.WrappedDriver.Wait(d => condition(element), timeout, exceptionTypes);
        }
        public static TElement WaitUntil<TElement>(this TElement element, Func<TElement, bool> condition, double timeout = -1, params Type[] exceptionTypes) where TElement : WebElement
        {
            element.Wait(condition, timeout, exceptionTypes);
            return element;
        }
        public static bool TryWait(this WebElement element, Func<WebElement, bool> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            try
            {
                return Wait(element, condition, timeout, exceptionTypes);
            }
            catch
            {
                return false;
            }
        }
        public static bool ShouldBe(this WebElement element, Func<WebElement, bool> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            return TryWait(element, condition, timeout, exceptionTypes);
        }
        public static bool ShouldNot(this WebElement element, Func<WebElement, bool> condition, double timeout = -1, params Type[] exceptionTypes)
        {
            return TryWait(element, e => !condition(e), timeout, exceptionTypes);
        }

        public static IEnumerable<T> LocateAll<T>(this IEnumerable<T> elements) where T : ILocate<T>
        {
            return elements.Select(e => e.Locate());
        }
    }
}