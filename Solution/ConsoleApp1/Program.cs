using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.PageObjects;
using WebDriverFramework;
using WebDriverFramework.Extension;
using WebDriverFramework.PageFactory;
using WebDriverFramework.PageFactory.Attributes;
using WebDriverFramework.Proxy;

namespace ConsoleApp1
{
    class Program
    {
        [ByXPath("//div[@class='test2']")]
        public IWebElement Test = null;

        [CacheLookup]
        [ByXPath("//div[@class='test2']")]
        private WebElement element1 = null;

        [RelateTo(nameof(element1))]
        [ByXPath("./label")]
        private WebElement element2 = null;

        [ByXPath("//input")]
        private WebElement element3 = null;

        [ByXPath("//label")]
        private IList<IWebElement> element4 = null;

        public Program(WebDriver driver)
        {
            CustomPageFactory.InitElements(this, driver, new CustomPageObjectMemberDecorator());
        }

        public static void AssertTrue(bool value)
        {
            if (!value)
            {
                throw new Exception("value is not true");
            }
        }

        public static void AssertFalse(bool value)
        {
            if (value)
            {
                throw new Exception("value is not false");
            }
        }
        static void Main(string[] args)
        {

            var t = new WebElement(By.XPath("asd"), (IWebDriver)null);
            t.Clear().SendKeys("asd").Submit().WaitUntil(Condition.NotExist);
            ((IWebElement)t).FindElement(By.XPath("ad"));
            var tt = (IWebElement) t;
            tt.WaitUntil(Condition.NotExist, 30);
            tt.WaitWhile(Condition.Exist);
            tt.TryWaitUntil(Condition.Exist, 30);
            if (t.Exist())
            {

            }
            tt.Clear();
            t.SendKeys("asd");
            Dictionary<int, int> dict = new Dictionary<int, int>()
            {
                {1, 2},
                {2, 3},
                {4, 2},
                {6, 5},
                {8, 3},
                {3, 5},
                {9, 3},
                {18, 4},
                {19, 4},
            };

            if (dict?.Any() != true)
            {

            }
            var all = dict.Keys.Concat(dict.Values).Distinct().ToList();
            var graph = new Graph<int>(dict, all);
            var rssesult = graph.BuildQueue();
            //IWebDriver _driver = new FirefoxDriver(new FirefoxOptions(){BrowserExecutableLocation = "C:\\Program Files\\Mozilla Firefox\\firefox.exe" });
            //IWebDriver _driver = new InternetExplorerDriver(new InternetExplorerOptions()
            //{
            //    IntroduceInstabilityByIgnoringProtectedModeSettings = true,
            //    IgnoreZoomLevel = true
            //});
            //WebElement.DefaultElementSearchTimeout = 60;

            var map = typeof(WebDriver).GetInterfaceMap(typeof(IWebDriver));
            var opt = new ChromeOptions();
            opt.AddArgument("disable-infobars");

            var _driver = new ChromeDriver(opt);
            var driver = new WebDriver(_driver);

            var z = driver.SwitchTo().Frame("asdasd").FindElement(".//asd");
            driver.Navigate().GoToUrl("file:///C:/Users/Artsiom_Kuis/Desktop/test.html");
            // driver.Navigate().GoToUrl("https://onliner.by");
            //driver.WaitForPresent(TimeSpan.FromSeconds(10), By.XPath(".//test"));
            var pr = new Program(driver);
            var p1 = new WebElementProxy(typeof(IWebElement), new DefaultElementLocator(driver), new[] { By.XPath(".//*") }, true);
            var tp1 = p1.GetTransparentProxy();
            p1.Locator = new DefaultElementLocator(driver);
            var tp2 = p1.GetTransparentProxy();
            Console.WriteLine(tp1 == tp2);
            //Console.WriteLine(p1.WrappedElement.TagName);
            var p2 = new WebElementProxy((IWebElement)p1.GetTransparentProxy());
            Console.WriteLine(pr.element2.Text);
            Console.WriteLine(p2.IsCached);
            Console.WriteLine();

            var elements = driver.Get("/*").GetAll();
            var zzzxczx = elements.Count;
            var test = elements.Locate();
            test.CheckStaleness();
            test.Locate();
            var test1 = elements.ToList();
            var rrrrr = pr.element1.Get(".").Locate();
            if (driver.WaitForPresent(By.XPath("some"), 10).Get("myxpath").Exist)
            {

            }

            ListWebElement l = new ListWebElement(By.XPath("wqe"), _driver);


            var elem = pr.element1.Get("asd").WaitUntil(Condition.Exist, 10).Get(".//test").Text;
            var result = pr.element1.Get(".//test").Locate();
            var ggg = pr.element1.Displayed;
            pr.element1 = new WebElement(By.XPath("//div[@class='test']"), driver.WrappedDriver);
            var vis = pr.element2.Text;
            var vis3 = pr.element3.Text;
            var vis4 = pr.element4.ToList();
            driver.Quit();
        }
    }

    public class Graph<T>
    {
        //dictonary - member and his parent 
        private Dictionary<T, T> relationDictionary;
        private List<T> allMembers;
        private HashSet<T> used = new HashSet<T>();

        public Graph(Dictionary<T, T> relationDictionary, List<T> allMembers)
        {
            this.relationDictionary = relationDictionary;
            this.allMembers = allMembers;
        }

        public List<T> BuildQueue()
        {
            var result = allMembers.SelectMany(Dfs).ToList();
            return result;
        }

        private List<T> Dfs(T member)
        {
            var currentChain = new List<T>();
            while (true)
            {
                //if it was already used or doesn't have a parent
                if (!used.Add(member) || !relationDictionary.ContainsKey(member))
                {
                    break;
                }

                currentChain.Add(member);
                member = relationDictionary[member];
            }

            currentChain.Reverse();
            return currentChain;
        }
    }
}
