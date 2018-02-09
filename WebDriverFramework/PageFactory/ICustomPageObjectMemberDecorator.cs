namespace WebDriverFramework.PageFactory
{
    using OpenQA.Selenium.Support.PageObjects;

    public interface ICustomPageObjectMemberDecorator : IPageObjectMemberDecorator
    {
        void FinishDecorate(object page);
    }
}
