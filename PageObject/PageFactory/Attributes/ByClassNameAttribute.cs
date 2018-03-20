using OpenQA.Selenium;

namespace PageObject.PageFactory.Attributes
{
    public class ByClassNameAttribute : ByAttribute
    {
        public ByClassNameAttribute(string className) : base(By.ClassName(className))
        {
        }
    }
}