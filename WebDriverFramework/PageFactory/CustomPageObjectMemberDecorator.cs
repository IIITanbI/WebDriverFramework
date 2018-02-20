namespace WebDriverFramework.PageFactory
{
    using Attributes;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using Proxy;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    public class CustomPageObjectMemberDecorator : ICustomPageObjectMemberDecorator
    {
        private readonly Dictionary<MemberInfo, object> _membersDictionary = new Dictionary<MemberInfo, object>();
        private readonly Dictionary<MemberInfo, DriverObjectProxy> _proxyDictionary = new Dictionary<MemberInfo, DriverObjectProxy>();

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
            return proxyObject;
        }

        private object DecorateObject(MemberInfo member, IElementLocator locator)
        {
            if (!(member is FieldInfo) && (member as PropertyInfo)?.CanWrite != true)
            {
                return null;
            }

            var bys = CreateLocatorList(member);
            if (!bys.Any())
            {
                return null;
            }

            var driver = (IWebDriver)locator.SearchContext;
            Type targetType = GetTargetType(member);
            bool cache = DefaultPageObjectMemberDecoratorProxy.ShouldCacheLookup(member);
            object result;

            DriverObjectProxy proxyElement = null;
            if (typeof(WebElement).IsAssignableFrom(targetType))
            {
                result = new WebElement(bys, driver) { ShouldCached = cache };
            }
            else if (typeof(IWebElement).IsAssignableFrom(targetType))
            {
                proxyElement = new WebElementProxy(typeof(IWebElement), locator, bys, cache);
                result = proxyElement.GetTransparentProxy();
            }
            else if (targetType == typeof(ListWebElement))
            {
                result = new ListWebElement(bys, driver);
            }
            else if (targetType == typeof(IList<IWebElement>))
            {
                proxyElement = new WebElementListProxy(typeof(IList<IWebElement>), locator, bys, cache);
                result = proxyElement.GetTransparentProxy();
            }
            else
            {
                throw new Exception($"Undefined type of element: '{targetType?.FullName}'");
            }

            _proxyDictionary.Add(member, proxyElement);
            _membersDictionary.Add(member, result);

            return result;

        }

        /// <summary>
        /// Creates a list of <see cref="By"/> locators based on the attributes of this member.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo"/> containing information about
        /// the member of the Page Object class.</param>
        /// <returns>A list of <see cref="By"/> locators based on the attributes of this member.</returns>
        private static List<By> CreateLocatorList(MemberInfo member)
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

            return bys;
        }

        public void FinishDecorate(object page)
        {
            foreach (var member in _membersDictionary.Keys)
            {
                if (!(member.GetCustomAttribute(typeof(RelateToAttribute)) is RelateToAttribute att))
                {
                    continue;
                }

                if (_membersDictionary[member] == null)
                {
                    throw new Exception($"member '{member}' has a RelateToAttribute but doesn't have any locators");
                }

                var name = att.FieldName;
                var matchList = _membersDictionary.Keys.Where(p => p.Name == name).ToList();
                if (matchList.Count != 1)
                {
                    throw new Exception($"There is no field with name '{name}' or their number more than 1");
                }

                var parentMember = matchList.First();
                var current = _membersDictionary[member];
                var parent = _membersDictionary[parentMember];

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
                        throw new NotImplementedException($"Type '{GetTargetType(parentMember)}' is not supported as parent");
                }

                switch (current)
                {
                    case ListWebElement _:
                    case WebElement ___:
                    case IWebElement __:
                        break;
                    default:
                        throw new NotImplementedException($"Type '{GetTargetType(member)}' is not supported as child for '{GetTargetType(parentMember)}'");
                }

                switch (current)
                {
                    case ListWebElement lwe when parent is WebElement pwe:
                        lwe.Parent = pwe;
                        break;
                    case WebElement we when parent is WebElement pwe:
                        we.Parent = pwe;
                        break;
                    default:
                        this._proxyDictionary[member].Locator = new DefaultElementLocator(context);
                        break;
                }
            }
        }

        private static Type GetTargetType(MemberInfo member) => (member as FieldInfo)?.FieldType ?? (member as PropertyInfo)?.PropertyType;

        private class DefaultPageObjectMemberDecoratorProxy : DefaultPageObjectMemberDecorator
        {
            public new static ReadOnlyCollection<By> CreateLocatorList(MemberInfo member) => DefaultPageObjectMemberDecorator.CreateLocatorList(member);

            public new static bool ShouldCacheLookup(MemberInfo member) => DefaultPageObjectMemberDecorator.ShouldCacheLookup(member);
        }
    }
}
