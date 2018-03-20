using OpenQA.Selenium;

namespace PageObject.PageFactory.Attributes
{
    public class ByXPathAttribute : ByAttribute
    {
        public ByXPathAttribute(string xpath) : base(By.XPath(xpath))
        {
        }
    }
}