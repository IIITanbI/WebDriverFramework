using System.Reflection;

namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;

    public class WebElement : IWebElement
    {
        private ProxyElement ProxyElement { get; }
        private IWebElement ImplicitElement { get; }
        private IWebDriver Driver { get; }

        public WebElement(ProxyElement proxyElement, IWebDriver driver) : this(proxyElement, null, driver)
        {
        }
        public WebElement(ProxyElement proxyElement, WebElement parent, IWebDriver driver) : this((By)null, parent, driver)
        {
            this.ProxyElement = proxyElement;
        }

        public WebElement(IWebElement implicitElement, IWebDriver driver) : this(implicitElement, null, driver)
        {
        }
        public WebElement(IWebElement implicitElement, WebElement parent, IWebDriver driver) : this((By)null, parent, driver)
        {
            this.ImplicitElement = implicitElement;
        }

        public WebElement(By locator, IWebDriver driver) : this(locator, null, driver)
        {
        }
        public WebElement(By locator, WebElement parent, IWebDriver driver)
        {
            this.Locator = locator;
            this.Parent = parent;
            this.Driver = driver;
        }

        public IWebElement Element => this.ImplicitElement
                                   ?? this.ProxyElement?.Element
                                   ?? this.Parent?.FindElement(Locator)
                                   ?? this.Driver.FindElement(Locator);
        public By Locator { get; }
        public WebElement Parent { get; }

        public bool IsProxy => this.ProxyElement != null;
        public bool IsImplicit => this.ImplicitElement != null;
        public bool IsCached
        {
            get
            {
                if (this.ProxyElement != null)
                {
                    return this.ProxyElement.Cache;
                }

                return this.ImplicitElement != null;
            }
        }

        public bool Exist
        {
            get
            {
                try
                {
                    StubActionOnElement();
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
        }

        public bool StubActionOnElement()
        {
            return this.Element.Displayed;
        }


        public WebElement Has(By locator)
        {
            return this.Has<WebElement>(locator);
        }
        public WebElement Has(IWebElement element)
        {
            return this.Has<WebElement>(element);
        }
        public T Has<T>(By locator) where T : WebElement
        {
            return (T)Activator.CreateInstance(typeof(T), locator, this.Driver);
        }
        public T Has<T>(IWebElement element) where T : WebElement
        {
            return (T)Activator.CreateInstance(typeof(T), element, this.Driver);
        }

        #region MyRegion
        public string TagName => Element.TagName;

        public string Text => TagName == "input" ? GetAttribute("value") : Element.Text;

        public bool Enabled => Element.Enabled;

        public bool Selected => Element.Selected;

        public Point Location => Element.Location;

        public Size Size => Element.Size;

        public bool Displayed => Element.Displayed;

        public void Clear()
        {
            Element.Clear();
        }

        public void SendKeys(string text)
        {
            Element.SendKeys(text);
        }

        public void Submit()
        {
            Element.Submit();
        }

        public void Click()
        {
            Element.Click();
        }

        public string GetAttribute(string attributeName)
        {
            return Element.GetAttribute(attributeName);
        }

        public string GetProperty(string propertyName)
        {
            return Element.GetProperty(propertyName);
        }

        public string GetCssValue(string propertyName)
        {
            return Element.GetCssValue(propertyName);
        }

        public IWebElement FindElement(By by)
        {
            return Element.FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            IEnumerable<IWebElement> collection = Element.FindElements(by).Select(this.Has);
            return collection.ToList().AsReadOnly();
        }
        #endregion

        #region Wait
        public MyWait GetWait(TimeSpan timeout, params Type[] exceptionTypes)
        {
            var wait = new MyWait(this.Driver, timeout);
            wait.IgnoreExceptionTypes(exceptionTypes);
            return wait;
        }

        public T Wait<T>(Func<T> condition, TimeSpan timeout, params Type[] exceptionTypes)
        {
            return GetWait(timeout, exceptionTypes).Until(condition);
        }
        public T Wait<T>(Func<IWebDriver, T> condition, TimeSpan timeout, params Type[] exceptionTypes)
        {
            return GetWait(timeout, exceptionTypes).Until(condition);
        }

        /// <summary>
        /// If it is proxy => call any property or function to get element from proxy (also check for stale reference, if proxy is cached) 
        /// If it is implicit => call any property or function to check element not stale
        /// if it is locator => find element by locator and call any property to check element is found
        /// If element is cached and StaleElementReferenceException was raised, throw this exception
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public WebElement WaitForElement(TimeSpan timeout)
        {
            Wait(() =>
            {
                try
                {
                    this.StubActionOnElement();
                    return true;
                }
                catch (Exception ex)
                {
                    switch (ResolveException(ex))
                    {
                        case StaleElementReferenceException s:
                            if (this.IsCached)
                            {
                                throw;
                            }
                            return false;
                        default:
                            throw;
                    }
                }
            }, timeout);

            return this;
        }

        /// <summary>
        /// If it is proxy => call any property or function to get element from proxy (also check for stale reference, if proxy is cached) 
        /// If it is implicit => call any property or function to check element not stale
        /// if it is locator => find element by locator and call any property to check element is found
        /// If element is cached and StaleElementReferenceException was raised, than to assume that element is not present
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public WebElement WaitForNoElement(TimeSpan timeout)
        {
            Wait(() =>
            {
                try
                {
                    this.StubActionOnElement();
                    return true;
                }
                catch (Exception ex)
                {
                    switch (ResolveException(ex))
                    {
                        case NoSuchElementException n:
                            return true;
                        case StaleElementReferenceException s:
                            if (this.IsCached)
                            {
                                throw;
                            }
                            return true;
                        default:
                            throw;
                    }
                }
            }, timeout);

            return this;
        }

        public WebElement WaitForElementDisplayed(TimeSpan timeout)
        {
            Wait(() =>
            {
                try
                {
                    return this.Element.Displayed;
                }
                catch (Exception ex)
                {
                    switch (ResolveException(ex))
                    {
                        case StaleElementReferenceException s:
                            if (this.IsCached)
                            {
                                throw;
                            }
                            return false;
                        default:
                            throw;
                    }
                }
            }, timeout);

            return this;
        }
        public WebElement WaitForElementNotDisplayed(TimeSpan timeout)
        {
            Wait(() =>
            {
                try
                {
                    return !this.Element.Displayed;
                }
                catch (Exception ex)
                {
                    switch (ResolveException(ex))
                    {
                        case NoSuchElementException n:
                        case StaleElementReferenceException s:
                            return true;
                        default:
                            throw;
                    }
                }
            }, timeout);

            return this;
        }

        public WebElement WaitForElementClickable(TimeSpan timeout)
        {
            Wait(() =>
            {
                try
                {
                    return this.Element.Enabled;
                }
                catch (Exception ex)
                {
                    switch (ResolveException(ex))
                    {
                        case StaleElementReferenceException s:
                            if (this.IsCached)
                            {
                                throw;
                            }
                            return false;
                        default:
                            throw;
                    }
                }
            }, timeout);

            return this;
        }

        private bool? ProcessException(Exception ex, bool noSuchElementException, bool staleElementReferenceException, bool throwOnstaleElementReferenceException)
        {
            switch (ResolveException(ex))
            {
                case NoSuchElementException nsee:
                    return noSuchElementException;
                case StaleElementReferenceException sere:
                    if (this.IsCached && throwOnstaleElementReferenceException)
                    {
                        return null;
                    }
                    return staleElementReferenceException;
                default:
                    return null;
            }
        }
        private static Exception ResolveException(Exception ex)
        {
            return (ex as TargetInvocationException)?.InnerException ?? ex;
        }

        #endregion
    }
}
