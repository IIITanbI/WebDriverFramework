namespace WebDriverFramework
{
    using System.Collections.Generic;

    public interface IElementList : IReadOnlyList<WebElement>
    {
        IElementList Locate();
        IElementList CheckStaleness();
    }
}
