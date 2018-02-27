namespace WebDriverFramework
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ElementListExtension
    {
        public static IEnumerable<T> LocateAll<T>(this IEnumerable<T> elements) where T : ILocate<T>
        {
            return elements.Select(e => e.Locate());
        }
    }
}