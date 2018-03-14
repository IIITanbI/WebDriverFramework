namespace WebDriverFramework
{
    using System;

    public static class ElementFactory
    {
        public static object Create(Type type, params object[] parameters) => Activator.CreateInstance(type, parameters);
        public static T Create<T>(params object[] parameters) => (T)Create(typeof(T), parameters);
    }
}