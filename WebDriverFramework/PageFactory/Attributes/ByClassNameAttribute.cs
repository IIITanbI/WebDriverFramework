namespace WebDriverFramework.PageFactory.Attributes
{
    using OpenQA.Selenium;

    public class ByClassNameAttribute : ByAttribute
    {
        public ByClassNameAttribute(string className) : base(By.ClassName(className))
        {
        }
    }
}