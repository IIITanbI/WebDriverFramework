using System.Collections.ObjectModel;

namespace WebDriverFramework.Extension
{
    using System;
    using System.Threading;
    using System.Windows.Forms;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Internal;
    using OpenQA.Selenium.Remote;
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
        public static IWebElement GetParent(this IWebElement element)
        {
            return element.FindElement(By.XPath("./.."));
        }

        #region Waiting
        #endregion

        public static bool IsElementCached(this IWebElement element)
        {
            switch (element)
            {
                case WebElement we:
                    return we.IsCached;
                case RemoteWebElement _:
                    return true;
                default:
                    //element can be proxy with cache = true/false
                    //assume that cache is false;
                    return false;
            }
        }

        public static IWebElement Unwrap(this IWebElement element)
        {
            var e = element;
            while (e is IWrapsElement)
            {
                e = (e as IWrapsElement).WrappedElement;
            }

            return e;
        }

        //public static void ExecuteJavaScript(this IWebElement element, string script, params object[] args)
        //{
        //    element.GetDriver().ExecuteJavaScript(script, args);
        //}
        //public static T ExecuteJavaScript<T>(this IWebElement element, string script, params object[] args)
        //{
        //    return element.GetDriver().ExecuteJavaScript<T>(script, args);
        //}

        public static IWebDriver GetDriver(this IWebElement element)
        {
            return (element.Unwrap() as IWrapsDriver)?.WrappedDriver ?? throw new NotImplementedException();
        }

        public static IWebElement FindElement(this IWebElement element, string xpath)
        {
            return element.FindElement(By.XPath(xpath));
        }
        public static ReadOnlyCollection<IWebElement> FindElements(this IWebElement element, string xpath)
        {
            return element.FindElements(By.XPath(xpath));
        }
    }
}
