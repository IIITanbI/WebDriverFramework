namespace WebDriverFramework.PageFactory
{
    using Attributes;
    using Elements;
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

        private readonly WebDriver _driver;

        public CustomPageObjectMemberDecorator(WebDriver driver)
        {
            this._driver = driver;
        }

        public object Decorate(MemberInfo member, IElementLocator locator)
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

            Type targetType = GetTargetType(member);
            bool cache = DefaultPageObjectMemberDecoratorProxy.ShouldCacheLookup(member);
            object result;

            DriverObjectProxy proxyElement = null;
            if (typeof(WebElement).IsAssignableFrom(targetType))
            {
                result = ElementFactory.Create(targetType, bys.First(), null, this._driver);
            }
            else if (targetType == typeof(IWebElement))
            {
                proxyElement = new WebElementProxy(targetType, locator, bys, cache);
                result = proxyElement.GetTransparentProxy();
            }
            else if (targetType == typeof(IList<IWebElement>))
            {
                proxyElement = new WebElementListProxy(targetType, locator, bys, cache);
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
                        context = w.ProxyElement;
                        break;
                    case IWebElement iw:
                        context = iw;
                        break;
                    default:
                        throw new NotImplementedException($"Type '{GetTargetType(parentMember)}' is not supported as parent");
                }

                switch (current)
                {
                    case WebElement ___:
                    case IWebElement __:
                    case IList<IWebElement> _:
                        break;
                    default:
                        throw new NotImplementedException($"Type '{GetTargetType(member)}' is not supported as child for '{GetTargetType(parentMember)}'");
                }

                switch (current)
                {
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
