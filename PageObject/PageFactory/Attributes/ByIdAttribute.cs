using OpenQA.Selenium;

namespace PageObject.PageFactory.Attributes
{
    public class ByIdAttribute : ByAttribute
    {
        public ByIdAttribute(string id) : base(By.Id(id))
        {
        }
    }
}