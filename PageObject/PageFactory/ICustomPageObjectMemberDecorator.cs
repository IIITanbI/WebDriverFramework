using OpenQA.Selenium.Support.PageObjects;

namespace PageObject.PageFactory
{
    public interface ICustomPageObjectMemberDecorator : IPageObjectMemberDecorator
    {
        void FinishDecorate(object page);
    }
}
