using NLog;
using NLog.Targets;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.PageObjects;
using PageObject.PageFactory;
using PageObject.PageFactory.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using WebDriverFramework;
using WebDriverFramework.Elements;
using WebDriverFramework.Extension;

namespace ConsoleApp1
{
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using System.Collections;
    using System.Diagnostics;

    public class Nunit2759Test
    {
        public enum ValuesEnum
        {
            Value1,
            Value2,
            Value3,
        }

        public class VariableValues : NUnitAttribute, IParameterDataSource
        {
            public int Count { get; set; }

            IEnumerable IParameterDataSource.GetData(IParameterInfo parameter)
            {
                for (var i = 0; i < Count; i++)
                {
                    yield return i.ToString();
                }
            }
        }

        [Test]
        public void Test2759ShouldRun([Values(1, 2, 3)] int a, [Values] ValuesEnum values)
        {
        }

        [Test]
        public void Test2759MustSkip([VariableValues(Count = 0)] string context, [Values] ValuesEnum values)
        {
        }
    }
    class Obj : IEquatable<Obj>
    {
        public int A = 3;

        public bool Equals(Obj other)
        {
            return A.Equals(A);
        }
        //public override bool Equals(object obj)
        //{
        //    return Equals(obj as Obj);
        //}
    }
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

        public static Logger logger = LogManager.GetCurrentClassLogger();

        static void Test1123()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Console.WriteLine("Executing {0}.{1}",
                              m.ReflectedType.Name, m.Name);
        }
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            Thread.Sleep(2000);

            var el1 = sw.Elapsed;

            sw.Stop();
            Thread.Sleep(2000);
            var el2 = sw.IsRunning;

            var l1 = new List<(string PathNames, string Name)>()
            {
                ("chrome", "test1"),
                ("firefox", "test1"),
                ("chrome", "test2"),
                ("chrome", "test3"),
                ("chrome", "test4"),
                ("chrome", "test5"),
            };

            var l2 = new List<(string, string)>()
            {
                ("chrome", "test1"),
                ("firefox", "test1"),
                ("chrome", "test2"),
            };

            var itemsToDelete = from test in l2
                                join prevTest in l1 on test.Item2 equals prevTest.Name
                                //where test.Item1.Equals(prevTest.Item1)
                                //where test.Item1 equals prevTest.Item1
                                select new { test, prevTest };

            //CollectionAssert.AreEqual(ints, ints2);


            Obj o1 = new Obj();
            Obj o2 = new Obj();
            var rr = object.Equals(o1, o2);

            Dictionary<string, bool> d1 = new Dictionary<string, bool>() {
                {"val1", false },
                {"val2", false },
                {"val3", true},
            };

            IEnumerable<KeyValuePair<string, bool>> ienmuberable = d1;

            Dictionary<string, bool> d2 = new Dictionary<string, bool>() {
                {"val1", false },
                {"val2", false },
                {"val3", !true},
            };

            //     CollectionAssert.AreEqual(d1.ToList(), d2.ToList());
            KeyValuePair<int, Obj> s1 = new KeyValuePair<int, Obj>(1, o1);
            KeyValuePair<int, Obj> s2 = new KeyValuePair<int, Obj>(2, o2);

            //var defaultc = EqualityComparer<KeyValuePair<int, Obj>>.Default;
            //var pairr = s1.Equals(s2);
            //var dictr = d1.SequenceEqual(d2);
            Console.WriteLine();
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

            var drv = new ChromeDriver();
            drv.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            // var to = drv.Manage().Timeouts().ImplicitWait;
            var driver = new WebDriver(drv);
            // driver.NativeDriver.Navigate().GoToUrl("file:///C:/Users/Artsiom_Kuis/Desktop/test1.html");
            driver.NativeDriver.Navigate().GoToUrl("https://onliner.by");

            //drv.NetworkConditions = new ChromeNetworkConditions()
            //{
            //    IsOffline = true,
            //    Latency = TimeSpan.FromMilliseconds(100),
            //    DownloadThroughput = 1000,
            //    UploadThroughput = 1000
            //};
            driver.NativeDriver.Navigate().GoToUrl("https://tut.by");
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

        //[Test]
        //public void TakingHTML2CanvasFullPageScreenshot()
        //{
        //    using (var driver = new ChromeDriver())
        //    {
        //        Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //        // driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);
        //        driver.Navigate().GoToUrl(@"https://www.automatetheplanet.com/full-page-screenshots-webdriver-html2canvas");
        //        IJavaScriptExecutor js = driver;
        //        var html2canvasJs = File.ReadAllText("html2canvas.min.txt");
        //        js.ExecuteScript(html2canvasJs);
        //        string generateScreenshotJS = @"function genScreenshot () {
        //                                 var canvasImgContentDecoded;

        //                                    html2canvas(document.body).then(canvas => {
        //                                        document.body.appendChild(canvas);
        //                                        window.canvasImgContentDecoded = canvas.toDataURL(""image/png"");   
        //                                     });
        //                                   }
        //                                genScreenshot();";
        //        js.ExecuteScript(generateScreenshotJS);
        //        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1000));
        //        wait.IgnoreExceptionTypes(typeof(InvalidOperationException));
        //        var pngContent = wait.Until(
        //            wd =>
        //            {
        //                var bitmap = js.ExecuteScript(@"
        //                            if (typeof canvasImgContentDecoded === 'undefined') return null;
        //                            return canvasImgContentDecoded;");
        //                return bitmap;
        //            }).ToString();

        //        pngContent = pngContent.Replace("data:image/png;base64,", string.Empty);
        //        byte[] data = Convert.FromBase64String(pngContent);
        //        Image image;
        //        using (var ms = new MemoryStream(data))
        //        {
        //            image = Image.FromStream(ms);
        //        }
        //        image.Save("new.png", ImageFormat.Png);
        //    }
        //}

        //[Test]
        //public void TakingScreenShot_Old()
        //{
        //    var options = new FirefoxOptions
        //    {
        //        BrowserExecutableLocation = @"C:\Program Files\Mozilla Firefox\firefox.exe"
        //    };

        //    var opt1 = new InternetExplorerOptions()
        //    {
        //        IgnoreZoomLevel = true
        //    };
        //    using (var driver = new InternetExplorerDriver(opt1))
        //    {
        //        Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //        // driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);
        //        driver.Navigate().GoToUrl(@"https://www.automatetheplanet.com/full-page-screenshots-webdriver-html2canvas");

        //        var bitmap = new WebDriver(driver).MakeScreenshot(null);
        //        bitmap.Save("old.png", ImageFormat.Png);
        //    }
        //}

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
        [TestCase]
        public void TestCase1()
        {
            for (int i = 0; i < e; i++)
            {
                Thread.Sleep(200);
                logger.Info("1 " + res);
            }
        }

        [TestCase]
        public void TestCase2()
        {
            for (int i = 0; i < e; i++)
            {
                Thread.Sleep(1000);
                logger.Info("2 " + res);
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
