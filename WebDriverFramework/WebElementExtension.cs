using OpenQA.Selenium.Internal;

namespace WebDriverFramework
{
    using System;
    using System.Threading;
    using System.Windows.Forms;
    using OpenQA.Selenium;
    using Keys = OpenQA.Selenium.Keys;

    /// <summary>
    /// <see cref="IWebElement"/> extension
    /// </summary>
    public static class WebElementExtension
    {
        /// <summary>
        /// Object for locking while using Clipboard.
        /// </summary>
        private static readonly object Lock = new object();

        /// <summary>
        /// The copy paste into element.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public static void CopyPaste(this IWebElement element, string text)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            lock (Lock)
            {
                var thread = new Thread(
                    () =>
                    {
                        element.Click();
                        element.SendKeys(Keys.LeftControl + "a");
                        Clipboard.SetText(text);
                        element.SendKeys(Keys.LeftControl + "v");
                    });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }
        }

        /// <summary>
        ///  Clear and Type text by chars
        /// </summary>
        /// <param name="element"><see cref="IWebElement"/> web control</param>
        /// <param name="text">new text</param>
        public static void SendChars(this IWebElement element, string text)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            foreach (var ch in text)
            {
                element.SendKeys(ch.ToString());
            }
        }

        /// <summary>
        ///  Clear and Type text
        /// </summary>
        /// <param name="element"><see cref="IWebElement"/> web control</param>
        /// <param name="text">new text</param>
        public static void ClearAndSetText(this IWebElement element, string text)
        {
            if (element == null)
            {
                throw new ElementNotVisibleException($"Element is not visible!");
            }

            element.Clear();
            element.SendKeys(text);
        }

        /// <summary>
        ///  Clear and Type text by chars
        /// </summary>
        /// <param name="element"><see cref="IWebElement"/> web control</param>
        /// <param name="text">new text</param>
        public static void ClearAndSetTextByChars(this IWebElement element, string text)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            element.Clear();
            element.SendChars(text);
        }

        /// <summary>
        /// Select text and set text by chars
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="text">
        /// text that should be typed
        /// </param>
        public static void SelectAndSetTextByChars(this IWebElement element, string text)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            element.SendKeys(Keys.LeftControl + "a");
            element.SendChars(text);
        }

        /// <summary>
        /// The get parent.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="IWebElement"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// throw if element is null
        /// </exception>
        public static IWebElement GetParent(this ISearchContext element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return element.FindElement(By.XPath("./.."));
        }


        public static MyWait GetWait(TimeSpan timeout, params Type[] exceptionTypes)
        {
            var wait = new MyWait(timeout);
            wait.IgnoreExceptionTypes(exceptionTypes);
            return wait;
        }
        public static T WaitT<T>(Func<T> condition, TimeSpan timeout, params Type[] exceptionTypes)
        {
            return GetWait(timeout, exceptionTypes).Until(condition);
        }

        public static T WaitForElementDisplayed<T>(this T element, TimeSpan timeout) where T : IWebElement
        {
            WaitT(() =>
            {
                try
                {
                    return element.Displayed;
                }
                catch (StaleElementReferenceException)
                {
                    if (element.CheckElementCached())
                    {
                        throw;
                    }

                    return false;
                }
            }, timeout);
            return element;
        }

        private static bool CheckElementCached(this IWebElement element)
        {
            switch (element)
            {
                case null:
                    throw new ArgumentNullException(nameof(element));
                case WebElement we:
                    return we.IsCached;
            }

            return true;
        }

        public static T WaitForElement123<T>(this T element, TimeSpan timeout) where T : IWebElement
        {
            // use any object's method or property (field) to unwrap it from proxy
            WaitT(() => element.GetType(), timeout);
            return element;
        }
    }
}
