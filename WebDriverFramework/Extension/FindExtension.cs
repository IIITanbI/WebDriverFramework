﻿namespace WebDriverFramework.Extension
{
    using Elements;
    using OpenQA.Selenium;
    using System.Collections.Generic;
    using System.Linq;

    public static class FindExtension
    {
        public static T Find<T>(this IFindElement ife, string xpath) where T : ILocate<T> => ife.Find<T>(By.XPath(xpath));
        public static T Find<T>(this IFindElement ife, By locator) where T : ILocate<T> => ife.Get<T>(locator).Locate();
        public static IList<T> FindAll<T>(this IFindElement ife, string xpath = ".//*") where T : ILocate<T> => ife.FindAll<T>(By.XPath(xpath));
        public static IList<T> FindAll<T>(this IFindElement ife, By locator) where T : ILocate<T> => ife.GetAll<T>(locator).LocateAll().ToList();

        public static T Get<T>(this IFindElement ife, string xpath) => ife.Get<T>(By.XPath(xpath));
        public static IEnumerable<T> GetAll<T>(this IFindElement ife, string xpath = ".//*") => ife.GetAll<T>(By.XPath(xpath));

        public static LabelElement Find(this IFindElement ife, string xpath) => ife.Find<LabelElement>(xpath);
        public static LabelElement Find(this IFindElement ife, By locator) => ife.Find<LabelElement>(locator);
        public static IList<LabelElement> FindAll(this IFindElement ife, string xpath = ".//*") => ife.FindAll<LabelElement>(xpath);
        public static IList<LabelElement> FindAll(this IFindElement ife, By locator) => ife.FindAll<LabelElement>(locator);

        public static LabelElement Get(this IFindElement ife, string xpath) => ife.Get<LabelElement>(xpath);
        public static LabelElement Get(this IFindElement ife, By locator) => ife.Get<LabelElement>(locator);
        public static IEnumerable<LabelElement> GetAll(this IFindElement ife, string xpath = ".//*") => ife.GetAll<LabelElement>(xpath);
        public static IEnumerable<LabelElement> GetAll(this IFindElement ife, By locator) => ife.GetAll<LabelElement>(locator);
    }
}