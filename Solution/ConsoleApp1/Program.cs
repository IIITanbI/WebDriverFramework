using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Common;
using NLog.Targets;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.PageObjects;
using ReportPortal.Client;
using ReportPortal.Shared;
using WebDriverFramework;
using WebDriverFramework.Elements;
using WebDriverFramework.Extension;
using WebDriverFramework.PageFactory;
using WebDriverFramework.PageFactory.Attributes;
using WebDriverFramework.Proxy;

namespace ConsoleApp1
{
    [Target("RP")]

    [TestFixture]
    class Program
    {
        [ByXPath("//div[@class='test2']")]
        public IWebElement Test = null;

        [CacheLookup]
        [ByXPath("//div[@class='test2']")]
        private LabelElement element1 = null;

        [RelateTo(nameof(element1))]
        [ByXPath("./label")]
        private LabelElement element2 = null;//=> element1.Find(".//test");

        [ByXPath("//input")]
        private LabelElement element3 = null;

        [ByXPath("//label")]
        private IList<IWebElement> element4 = null;

        public Program(WebDriver driver)
        {
            CustomPageFactory.InitElements(this, driver.WrappedDriver, new CustomPageObjectMemberDecorator());
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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void button1()

        {
            var temp = testAsync();
            Debug.WriteLine("pass");
            var value = temp.Result; //Blocking forever....
        }

        private async Task<bool> testAsync()
        {
            return await Task.Run(() =>
            {
                Debug.WriteLine("start wait one");
                Debug.WriteLine("reset");
                return true; //Code goes here without any error.....
            });
        }

        static void Main(string[] args)
        {

            Bridge.Service = new Service(new Uri("http://localhost:8080/api/v1/"), "myproject", "7a580212-5f06-4f46-8cc3-3f78cc4aa282");
            Bridge.Service = new Service(new Uri("https://rp.epam.com/api/v1/"), "ARTSIOM_KUIS_PERSONAL", "591b2176-229c-4d3e-aca8-bbd02e9e5e55");
            //591b2176-229c-4d3e-aca8-bbd02e9e5e55
            var launch = Bridge.Service.StartLaunchAsync(new ReportPortal.Client.Requests.StartLaunchRequest()
            {
                Name = "new launch",
                Description = "asd",
                StartTime = DateTime.Now,
            }).Result;


            var test = Bridge.Service.StartTestItemAsync(new ReportPortal.Client.Requests.StartTestItemRequest()
            {
                LaunchId = launch.Id,
                Name = "testItem",
                Description = "sadsad",
                StartTime = DateTime.Now,
            }).Result;

            var tasks = new List<Task>();
            for (int i = 0; i < 1e5; i++)
            {
                tasks.Add(new Task(async () =>
                {
                    await Bridge.Service.AddLogItemAsync(new ReportPortal.Client.Requests.AddLogItemRequest()
                    {
                        TestItemId = test.Id,
                        Level = ReportPortal.Client.Models.LogLevel.Info,
                        Text = "sadkdsasalkjd",
                        Time = DateTime.Now
                    });
                }));
            }
            tasks.ForEach(t => t.Start());
            Task.WaitAll(tasks.ToArray());

            Bridge.Service.FinishTestItemAsync(test.Id, new ReportPortal.Client.Requests.FinishTestItemRequest()
            {
                EndTime = DateTime.Now,
                Status = ReportPortal.Client.Models.Status.Passed
            }).Wait();

            Bridge.Service.FinishLaunchAsync(launch.Id, new ReportPortal.Client.Requests.FinishLaunchRequest()
            {
                EndTime = DateTime.Now
            }).Wait();

            Console.WriteLine("END!!!!");
            Console.ReadLine();
            // var profile = new FirefoxProfile();
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

            var all = dict.Keys.Concat(dict.Values).Distinct().ToList();
            var graph = new Graph<int>(dict, all);
            //IWebDriver _driver = new FirefoxDriver(new FirefoxOptions(){BrowserExecutableLocation = "C:\\Program Files\\Mozilla Firefox\\firefox.exe" });
            //IWebDriver _driver = new InternetExplorerDriver(new InternetExplorerOptions()
            //{
            //    IntroduceInstabilityByIgnoringProtectedModeSettings = true,
            //    IgnoreZoomLevel = true
            //});
            //WebElement.DefaultElementSearchTimeout = 60;

            var opt = new ChromeOptions();
            opt.AddArgument("disable-infobars");

            var _driver = new ChromeDriver(opt);
            var driver = new WebDriver(_driver);

            //driver.WrappedDriver.Navigate().GoToUrl("file:///C:/Users/Artsiom_Kuis/Desktop/test.html");
            driver.WrappedDriver.Navigate().GoToUrl("https://onliner.by");
            var rssesult = driver.WrappedDriver.ExecuteJavaScript<string>("return window.localStorage");
            var rssessult = driver.WrappedDriver.ExecuteJavaScript<string>("window.localStorage.setItem('key', 'value')");
            //driver.WaitForPresent(TimeSpan.FromSeconds(10), By.XPath(".//test"));
            var label = new CheckBox(By.XPath("asdsad"), null, driver.WrappedDriver).WaitUntil(ch => ch.Selected);
            var checkbox = new LabelElement(By.XPath("asdsad"), null, driver.WrappedDriver);
            var c1eckbox = ElementFactory.Create<CheckBox>(By.XPath("asdsad"), null, driver.WrappedDriver);
            var z = checkbox.Wait(elemet => elemet.Displayed);
            label.Click();
            checkbox.Click();
            var result1s = checkbox.Locate();
            driver.Find("//input[@id='input']").Click();

            var pr = new Program(driver);
            var p1 = new WebElementProxy(typeof(IWebElement), new DefaultElementLocator(driver.WrappedDriver), new[] { By.XPath(".//*") }, true);
            var tp1 = p1.GetTransparentProxy();
            p1.Locator = new DefaultElementLocator(driver.WrappedDriver);
            var tp2 = p1.GetTransparentProxy();
            Console.WriteLine(tp1 == tp2);
            //Console.WriteLine(p1.WrappedElement.TagName);
            var p2 = new WebElementProxy((IWebElement)p1.GetTransparentProxy());
            Console.WriteLine(pr.element2.Text);
            Console.WriteLine(p2.IsCached);
            Console.WriteLine();

            //var elements = driver.Get("/*").FindAll();
            //var zzzxczx = elements.Count;
            //var test = (elements).LocateAll();
            //var test1 = elements.ToList();
            //var rrrrr = pr.element1.Get(".").Locate();

            //var el = driver.Get<CheckBox>("//some").WaitUntil(Condition.Exist);
            //var el1 = driver.Get("//some").WaitUntil(Condition.Exist);



            //var elem = pr.element1.Get("asd").WaitUntil(Condition.Exist, 10).Get(".//test").Text;
            //var result = pr.element1.Get(".//test").Locate();
            //var ggg = pr.element1.Displayed;
            //pr.element1 = new LabelElement(By.XPath("//div[@class='test']"), null, driver.WrappedDriver);
            //var vis = pr.element2.Text;
            //var vis3 = pr.element3.Text;
            //var vis4 = pr.element4.ToList();
            //driver.WrappedDriver.Quit();
        }

        public Program()
        {

        }



        [TestCase]
        public void TestCase1()
        {
            logger.Info("lol");
        }

        [TestCase]
        public void TestCase()
        {
            logger.Info("asdasd" + "{rp#file#" + @"C:\Users\Artsiom_Kuis\Desktop\test.html}");
            logger.Info("lol");
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
