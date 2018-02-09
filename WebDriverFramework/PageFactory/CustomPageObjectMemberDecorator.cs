namespace WebDriverFramework.PageFactory
{
    using Attributes;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using Proxy;

    public class CustomPageObjectMemberDecorator : ICustomPageObjectMemberDecorator
    {
        private readonly Dictionary<MemberInfo, object> _membersDictionary = new Dictionary<MemberInfo, object>();

        private IWebDriver _driver;
        public CustomPageObjectMemberDecorator(IWebDriver driver)
        {
            this._driver = driver;
        }

        /// <summary>
        /// Locates an element or list of elements for a Page Object member, and returns a
        /// proxy object for the element or list of elements.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo"/> containing information about
        /// a class's member.</param>
        /// <param name="locator">The <see cref="IElementLocator"/> used to locate elements.</param>
        /// <returns>A transparent proxy to the WebDriver element object.</returns>
        public object Decorate(MemberInfo member, IElementLocator locator)
        {
            object proxyObject = DecorateObject(member, locator);
            _membersDictionary.Add(member, proxyObject);
            return proxyObject;
        }

        private object DecorateObject(MemberInfo member, IElementLocator locator)
        {
            FieldInfo field = member as FieldInfo;
            PropertyInfo property = member as PropertyInfo;

            if (field == null && (property == null || !property.CanWrite))
            {
                return null;
            }

            Type targetType = GetTargetType(member);

            IList<By> bys = CreateLocatorList(member);
            if (bys.Any())
            {
                bool cache = DefaultPageObjectMemberDecoratorProxy.ShouldCacheLookup(member);
                object result;

                if (typeof(WebElement).IsAssignableFrom(targetType))
                {
                    var proxyElement = new WebElementProxy(typeof(IWebElement), locator, bys, cache);
                    var args = new object[] { proxyElement, this._driver };
                    result = Activator.CreateInstance(targetType, args);
                }
                else if (typeof(IWebElement).IsAssignableFrom(targetType))
                {
                    result = new WebElementProxy(typeof(IWebElement), locator, bys, cache).GetTransparentProxy();
                }
                else if (typeof(IList<WebElement>).IsAssignableFrom(targetType))
                {
                    throw new NotImplementedException();
                    //var proxyElements = new ProxyListElement(locator, bys, cache);
                    //var args = new object[] { proxyElements, this._driver };
                    //result = Activator.CreateInstance(targetType, args);
                }
                else if (targetType == typeof(IList<IWebElement>))
                {
                    result = new WebElementListProxy(typeof(IList<IWebElement>), locator, bys, cache).GetTransparentProxy();
                }
                else
                {
                    throw new Exception($"Undefined type of element: '{targetType?.FullName}'");
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// Creates a list of <see cref="By"/> locators based on the attributes of this member.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo"/> containing information about
        /// the member of the Page Object class.</param>
        /// <returns>A list of <see cref="By"/> locators based on the attributes of this member.</returns>
        private static ReadOnlyCollection<By> CreateLocatorList(MemberInfo member)
        {
            List<By> bys = DefaultPageObjectMemberDecoratorProxy.CreateLocatorList(member).ToList();

            var allAttributes = member.GetCustomAttributes(true);
            var useSequence = allAttributes.OfType<FindsBySequenceAttribute>().Any();
            var useAll = allAttributes.OfType<FindsByAllAttribute>().Any();

            var attributes = allAttributes.OfType<ByAttribute>().ToList();
            if (attributes.Any())
            {
                bys.AddRange(attributes.Select(attribute => attribute.Locator));

                if (useSequence)
                {
                    bys = new List<By>() { new ByChained(bys.ToArray()) };
                }
                else if (useAll)
                {
                    bys = new List<By>() { new ByAll(bys.ToArray()) };
                }
            }

            return bys.AsReadOnly();
        }

        public void FinishDecorate(object page)
        {
            foreach (var member in _membersDictionary.Keys)
            {
                if (!(member.GetCustomAttribute(typeof(RelateToAttribute)) is RelateToAttribute att))
                {
                    continue;
                }

                var name = att.FieldName;
                var matchList = _membersDictionary.Keys.Where(p => p.Name == name).ToList();
                if (matchList.Count != 1)
                {
                    throw new Exception($"There is no field with name '{name}' or their number more than 1");
                }

                var parent = _membersDictionary[matchList.First()];

                ISearchContext context;
                switch (parent)
                {
                    case WebElement w:
                        context = w.WrappedElement;
                        break;
                    case IWebElement iw:
                        context = iw;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                var decoratedValue = DecorateObject(member, new DefaultElementLocator(context));
                if (member is FieldInfo field)
                {
                    field.SetValue(page, decoratedValue);
                }
                else
                {
                    ((PropertyInfo)member).SetValue(page, decoratedValue);
                }
            }
        }

        private static Type GetTargetType(MemberInfo member)
        {
            Type targetType = (member as FieldInfo)?.FieldType
                           ?? (member as PropertyInfo)?.PropertyType;
            return targetType;
        }

        private class DefaultPageObjectMemberDecoratorProxy : DefaultPageObjectMemberDecorator
        {
            public new static ReadOnlyCollection<By> CreateLocatorList(MemberInfo member) => DefaultPageObjectMemberDecorator.CreateLocatorList(member);

            public new static bool ShouldCacheLookup(MemberInfo member) => DefaultPageObjectMemberDecorator.ShouldCacheLookup(member);
        }
    }
}
