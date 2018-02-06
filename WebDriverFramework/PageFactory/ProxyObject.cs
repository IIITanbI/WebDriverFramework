namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ProxyObject
    {
        public Type TypeToBeProxied { get; }
        public IElementLocator Locator { get; }
        public IEnumerable<By> Bys { get; }
        public bool Cache { get; }

        public ProxyObject(Type typeToBeProxied, IElementLocator locator, IEnumerable<By> bys, bool cache)
        {
            this.TypeToBeProxied = typeToBeProxied;
            this.Cache = cache;
            this.Bys = bys;
            this.Locator = locator;
        }

        public object CreateProxiedObject()
        {
            var method = typeof(DefaultPageObjectMemberDecorator).GetMethod("CreateProxyObject", BindingFlags.Static | BindingFlags.NonPublic);
            var proxyObject = method.Invoke(null, new object[] { this.TypeToBeProxied, this.Locator, this.Bys, this.Cache });
            return proxyObject;
        }
    }

    public class ProxyElement : ProxyObject
    {
        public IWebElement Element { get; }

        public ProxyElement(IElementLocator locator, IEnumerable<By> bys, bool cache) : base(typeof(IWebElement), locator, bys, cache)
        {
            this.Element = (IWebElement)this.CreateProxiedObject();
        }
    }

    public class ProxyListElement : ProxyObject
    {
        public IList<IWebElement> Elements { get; }

        public ProxyListElement(IElementLocator locator, IEnumerable<By> bys, bool cache) : base(typeof(IList<IWebElement>), locator, bys, cache)
        {
            this.Elements = (IList<IWebElement>)this.CreateProxiedObject();
        }
    }
}