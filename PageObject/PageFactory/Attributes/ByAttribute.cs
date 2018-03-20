using System;
using OpenQA.Selenium;

namespace PageObject.PageFactory.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public abstract class ByAttribute : Attribute
    {
        protected ByAttribute(By locator)
        {
            this.Locator = locator;
        }

        public By Locator { get; set; }
    }
}
