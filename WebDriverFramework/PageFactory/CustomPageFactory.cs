namespace WebDriverFramework.PageFactory
{
    using OpenQA.Selenium.Support.PageObjects;
    using OpenQA.Selenium;

    public static class CustomPageFactory
    {
        /// <summary>
        /// Initializes the elements in the Page Object.
        /// </summary>
        /// <param name="driver">The driver used to find elements on the page.</param>
        /// <param name="page">The Page Object to be populated with elements.</param>
        /// <param name="decorator">The <see cref="OpenQA.Selenium.Support.PageObjects.IPageObjectMemberDecorator"/> implementation that
        /// determines how Page Object members representing elements are discovered and populated.</param>
        /// <exception cref="ArgumentException">
        /// thrown if a field or property decorated with the <see cref="OpenQA.Selenium.Support.PageObjects.FindsByAttribute"/> is not of type
        /// <see cref="IWebElement"/> or IList{IWebElement}.
        /// </exception>
        public static void InitElements(object page, IWebDriver driver, IPageObjectMemberDecorator decorator)
        {
            InitElements(page, new DefaultElementLocator(driver), decorator);
        }

        /// <summary>
        /// Initializes the elements in the Page Object.
        /// </summary>
        /// <param name="page">The Page Object to be populated with elements.</param>
        /// <param name="locator">The <see cref="IElementLocator"/> implementation that
        /// determines how elements are located.</param>
        /// <param name="decorator">The <see cref="IPageObjectMemberDecorator"/> implementation that
        /// determines how Page Object members representing elements are discovered and populated.</param>
        /// <exception cref="ArgumentException">
        /// thrown if a field or property decorated with the <see cref="OpenQA.Selenium.Support.PageObjects.FindsByAttribute"/> is not of type
        /// <see cref="IWebElement"/> or IList{IWebElement}.
        /// </exception>
        public static void InitElements(object page, IElementLocator locator, IPageObjectMemberDecorator decorator)
        {
            PageFactory.InitElements(page, locator, decorator);
            FinishDecorate(page, decorator);
        }

        private static void FinishDecorate(object page, IPageObjectMemberDecorator decorator)
        {
            if (decorator is ICustomPageObjectMemberDecorator custom)
            {
                custom.FinishDecorate(page);
            }
        }
    }
}
