namespace WebDriverFramework.PageFactory.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class RelateToAttribute : Attribute
    {
        public RelateToAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }

        public  string FieldName { get; set; }
    }
}