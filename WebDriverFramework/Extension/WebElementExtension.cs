using System.Collections.ObjectModel;
using WebDriverFramework.Proxy;

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
    public static partial class WebElementExtension
    {
        /// <summary>
        /// Object for locking while using Clipboard.
        /// </summary>
        private static readonly object Lock = new object();

        public static void CopyPaste(this IWebElement element, string text)
        {
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
        public static void SendByChars(this IWebElement element, string text)
        {
            foreach (var ch in text)
            {
                element.SendKeys(ch.ToString());
            }
        }
        public static void ClearAndSetText(this IWebElement element, string text)
        {
            if (element == null)
            {
                throw new ElementNotVisibleException($"Element is not visible!");
            }

            element.Clear();
            element.SendKeys(text);
        }
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
            element.SendByChars(text);
        }
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
            element.SendByChars(text);
        }

        public static bool Exist(this IWebElement element)
        {
            try
            {
                element.StubActionOnElement();
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        }

        //public static bool IsCached(this IWebElement element)
        //{
        //    switch (element)
        //    {
        //        case RemoteWebElement _:
        //            return true;
        //        case IProxiable proxy:
        //            return proxy.IsCached;
        //        default:
        //            return false;
        //    }
        //}

        public static IWebElement Unwrap(this IWebElement element) => Unwrap(element as IWrapsElement);
        public static IWebElement Unwrap(this IWrapsElement wrapsElement)
        {
            IWebElement result = null;
            IWrapsElement e = wrapsElement;
            while (e != null)
            {
                result = e.WrappedElement;
                e = result as IWrapsElement;
            }

            return result;
        }

        public static IWebDriver GetDriver(this IWebElement element)
        {
            return element.Unwrap() is IWrapsDriver wrap ? wrap.WrappedDriver : throw new NotImplementedException();
        }

        public static T StubActionOnElement<T>(this T element) where T : IWebElement
        {
            var tagName = element.TagName;
            return element;
        }

        public static T GetTParent<T>(this ISearchContext<T> element)
        {
            return element.FindElement(By.XPath("./.."));
        }
        public static IWebElement GetParent(this ISearchContext element)
        {
            return element.FindElement(By.XPath("./.."));
        }
        public static IWebElement FindElement(this ISearchContext context, string xpath)
        {
            return context.FindElement(By.XPath(xpath));
        }
        public static ReadOnlyCollection<IWebElement> FindElements(this ISearchContext context, string xpath)
        {
            return context.FindElements(By.XPath(xpath));
        }
    }
}
