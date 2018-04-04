using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.PageObjects;
using ReportPortal.Client;
using ReportPortal.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using NUnit.Framework;
using OpenQA.Selenium.IE;
using PageObject.PageFactory;
using PageObject.PageFactory.Attributes;
using ReportPortal.NUnitExtension;
using WebDriverFramework;
using WebDriverFramework.Elements;
using WebDriverFramework.Extension;

namespace ConsoleApp1
{
    [Target("RP")]
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
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
            CustomPageFactory.InitElements(this, driver.NativeDriver, new CustomPageObjectMemberDecorator(driver));
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
        public static Logger logger = LogManager.GetCurrentClassLogger();


        class Base
        {
            public int a = 3;
            public int b = 5;
            public int c;

            public Base()
            {
                c = a + b;
            }
        }
        class Derived:Base
        {
            public int d = 6;
         

            public Derived() : base()
            {
                c = a + d;
            }
        }

        class DerivedMore : Derived
        {
            public int x = 16;


            public DerivedMore() : base()
            {
                c = x + d;
            }
        }
        static void Main(string[] args)
        {
            var x = new DerivedMore();
            //Bridge.Service = new Service(new Uri("http://localhost:8080/api/v1/"), "myproject", "7a580212-5f06-4f46-8cc3-3f78cc4aa282");
            //Bridge.Service = new Service(new Uri("https://rp.epam.com/api/v1/"), "ARTSIOM_KUIS_PERSONAL", "591b2176-229c-4d3e-aca8-bbd02e9e5e55");
            ////591b2176-229c-4d3e-aca8-bbd02e9e5e55
            //var launch = Bridge.Service.StartLaunchAsync(new ReportPortal.Client.Requests.StartLaunchRequest()
            //{
            //    Name = "new launch",
            //    Description = "asd",
            //    StartTime = DateTime.Now,
            //}).Result;


            //var test = Bridge.Service.StartTestItemAsync(new ReportPortal.Client.Requests.StartTestItemRequest()
            //{
            //    LaunchId = launch.Id,
            //    Name = "testItem",
            //    Description = "sadsad",
            //    StartTime = DateTime.Now,
            //}).Result;

            //var time = DateTime.Now;
            //var tasks = new List<Task>();
            //for (int i = 1; i <= 1; i++)
            //{
            //    var i1 = i;
            //    //tasks.Add(new Task(async () =>
            //    //{
            //    //    await Task.Delay(1000 * 10);
            //    //    await Bridge.Service.AddLogItemAsync(new ReportPortal.Client.Requests.AddLogItemRequest()
            //    //    {
            //    //        TestItemId = test.Id,
            //    //        Level = ReportPortal.Client.Models.LogLevel.Info,
            //    //        Text = i1.ToString(),
            //    //        Time = time.AddMinutes(i1)
            //    //    });
            //    //}));


            //    tasks.Add(Bridge.Service.AddLogItemAsync(new ReportPortal.Client.Requests.AddLogItemRequest()
            //    {
            //        TestItemId = test.Id,
            //        Level = ReportPortal.Client.Models.LogLevel.Info,
            //        Text = i1.ToString(),
            //        Time = time.AddMinutes(i1)
            //    }));
            //}
            //tasks.ForEach(t => t.Start());
            //Task.WaitAll(tasks.ToArray());

            //var non = tasks.Where(t => t.Status != TaskStatus.RanToCompletion).ToList();
            //Bridge.Service.FinishTestItemAsync(test.Id, new ReportPortal.Client.Requests.FinishTestItemRequest()
            //{
            //    EndTime = DateTime.Now,
            //    Status = ReportPortal.Client.Models.Status.Passed
            //}).Wait();

            //Bridge.Service.FinishLaunchAsync(launch.Id, new ReportPortal.Client.Requests.FinishLaunchRequest()
            //{
            //    EndTime = DateTime.Now
            //}).Wait();

            //Console.WriteLine("END!!!!");
            //Console.ReadLine();
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

            //var all = dict.Keys.Concat(dict.Values).Distinct().ToList();
            //var graph = new Graph<int>(dict, all);
            //IWebDriver _driver = new FirefoxDriver(new FirefoxOptions(){BrowserExecutableLocation = "C:\\Program Files\\Mozilla Firefox\\firefox.exe" });
            //IWebDriver _driver = new InternetExplorerDriver(new InternetExplorerOptions()
            //{
            //    IntroduceInstabilityByIgnoringProtectedModeSettings = true,
            //    IgnoreZoomLevel = true
            //});
            //WebElement.DefaultElementSearchTimeout = 60;

            var opt = new ChromeOptions();
            opt.AddExcludedArgument("enable-automation");
            var driver = new WebDriver(new ChromeDriver(opt));

            driver.NativeDriver.Navigate().GoToUrl("file:///C:/Users/Artsiom_Kuis/Desktop/test1.html");
            //driver.Driver.Navigate().GoToUrl("https://onliner.by");
            //var rssesult = driver.Driver.ExecuteJavaScript<string>("return window.localStorage");
            //var rssessult = driver.Driver.ExecuteJavaScript<string>("window.localStorage.setItem('key', 'value')");
            //driver.WaitForPresent(TimeSpan.FromSeconds(10), By.XPath(".//test"));

            var label = driver.Get("./*");
            var ll = label.GetAll();
            var ll1 = ll.ToList();
            var ll2 = ll.ToList();
            var frame1 = driver.Get<FrameElement>("//*[@id='1']");
            var frame2 = driver.Get<FrameElement>("//*[@id='2']");
            var frame3 = frame1.Get<FrameElement>("//*[@id='3']");
            var label_1 = frame1.Get(".//*[@class='test2']").Locate();
            var label_2 = frame2.Get(".//*[@class='test4']").Locate();
            var label_3 = frame3.Get(".//*[@class='test5']").Locate();

            driver.Quit();
            return;
            var checkbox = new LabelElement(By.XPath("asdsad"), null, driver);
            var c1eckbox = ElementFactory.Create<CheckBox>(By.XPath("asdsad"), null, driver.NativeDriver);
            var z = checkbox.Wait(elemet => elemet.Displayed);
            checkbox.Click();
            var result1s = checkbox.Locate();
            driver.Find("//input[@id='input']").Click();

            var pr = new Program(driver);

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


        [SetUp]
        public void Setup()
        {
            //throw new ArgumentException();
            // new TestReporter(null, null, null).Start(null);
            //new ReportPortalListener().OnTestEvent("<XML></XML>");
            res = 3;
            logger.Info("Setup Method ");
            //Console.WriteLine("SETUP STRING");
        }

        [TearDown]
        public void Down()
        {
            res = 0;
            logger.Info("Down");
        }
        private object res = null;

        private int e = (int)10;
        // [TestCase]
        public void TestCase1()
        {
            for (int i = 0; i < e; i++)
            {
                Thread.Sleep(200);
                logger.Info("1" + res);
            }
        }

        // [TestCase]
        public void TestCase2()
        {
            for (int i = 0; i < e; i++)
            {
                Thread.Sleep(1000);
                logger.Info("2");
            }
        }
    }

    class TestProgramm : Program
    {
        [SetUp]
        public void Setup1()
        {
            Thread.Sleep(1000);
            logger.Info("Setup Method 1");
        }

        [SetUp]
        public void Setup2()
        {
            Thread.Sleep(1000);
            logger.Info("Setup Method 2");
        }
        [TearDown]
        public void Down1()
        {
            Thread.Sleep(1000);
            logger.Info("Down 1");
        }

        [TestCase]
        public void TestCase3()
        {
            logger.Warn("case");
            Thread.Sleep(1000);
            for (int i = 0; i < 10; i++)
            {
                //Thread.Sleep(100);
            }
        }

        [TestCase]
        public void TestCase4()
        {
            Thread.Sleep(200);
            for (int i = 0; i < 10; i++)
            {
                // Thread.Sleep(100);
            }
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
