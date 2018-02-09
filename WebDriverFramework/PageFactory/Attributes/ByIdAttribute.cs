namespace WebDriverFramework.PageFactory.Attributes
{
    using OpenQA.Selenium;

    public class ByIdAttribute : ByAttribute
    {
        public ByIdAttribute(string id) : base(By.Id(id))
        {
        }
    }
}