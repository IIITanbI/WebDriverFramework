namespace WebDriverFramework.Elements
{
    public interface IMyWebElement : IElement
    {
        IMyWebElement Parent { get; }
        WebDriver Driver { get; }
    }
}