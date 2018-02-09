namespace WebDriverFramework.PageFactory.Attributes
{
    using OpenQA.Selenium;

    public class ByXPathAttribute : ByAttribute
    {
        public ByXPathAttribute(string xpath) : base(By.XPath(xpath))
        {
        }
    }
}