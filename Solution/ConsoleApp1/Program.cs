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
        [FindsBy(How = How.XPath, Using = "//div[@class='test']")]
        [ByXPath("//div[@class='test']")]
        public IWebElement Test = null;

        [CacheLookup]
        [ByXPath("//div[@class='test']")]
        private WebElement element1 = null;

        [RelateTo(nameof(element1))]
        [ByXPath("./div[@class='test1']/label")]
        private WebElement element2 = null;

        [ByXPath("//input")]
        private WebElement element3 = null;

        [ByXPath("//label")]
        private IList<IWebElement> element4 = null;

        public Program(WebDriver driver)
        {
            CustomPageFactory.InitElements(this, driver, new CustomPageObjectMemberDecorator(driver.WrappedDriver));
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
            driver.Navigate().GoToUrl("file:///C:/Users/Artsiom_Kuis/Desktop/test.html");
            // driver.Navigate().GoToUrl("https://onliner.by");
            //driver.WaitForPresent(TimeSpan.FromSeconds(10), By.XPath(".//test"));
            var pr = new Program(driver);
            var p1 = new WebElementProxy(typeof(IWebElement), new DefaultElementLocator(driver), new[] {By.XPath(".//*")}, true);
            //Console.WriteLine(p1.WrappedElement.TagName);
            var p2 = new WebElementProxy((IWebElement)p1.GetTransparentProxy());

            Console.WriteLine(p2.IsCached);
            Console.WriteLine();
            // Console.WriteLine(driver.GetImplicitWait());
            {
                {
                    var sw = Stopwatch.StartNew();
                    driver.Get("sadlkjaslkjd").Wait(Condition.Displayed, 20);
                    var exi2t = driver.Get(".//*[@class='test']").Locate().WaitUntil(Condition.NotExist);
                    var exi3t = driver.Get("sadlkjaslkjd").Wait(Condition.Exist, 20);
                    var exiat = driver.Get("sadlkjaslkjd");
                    var exsiat = driver.Get("sadlkjaslkjd").ShouldBe(Condition.Exist);
                    var exs2at = driver.Get("sadlkjaslkjd").ShouldNot(Condition.Exist);
                    // var eaiatj = driver.Get("sadlkjaslkjd").ShouldBe(e => e.Exist);
                    AssertTrue(exiat.Displayed);
                    // AssertTrue(exiat.ShouldBe(e => e.Exist, 20));
                    // AssertTrue(exiat.ShouldBe(Condition.NotExist, 20));
                    sw.Stop();
                    Console.WriteLine("lvl 1 :" + sw.ElapsedMilliseconds);
                }
                Console.WriteLine(driver.GetImplicitWait());
                {
                    {
                        var sw = Stopwatch.StartNew();
                        var exist = driver.Get("sadlkjaslkjd").WaitUntil(Condition.Exist, 10).Exist;
                        var isexist = driver.Get("sadlkjaslkjd").Wait(Condition.Exist, 10);
                        var issist = driver.Get("sadlkjaslkjd").WaitUntil(Condition.Exist, 10);
                        var is1ist = driver.Get("sadlkjaslkjd").WaitUntil(zl => zl.Text == "test", 10);


                        var isesist = driver.Get("sadlkjaslkjd").TryWait(ee => ee.Exist);
                        sw.Stop();
                        Console.WriteLine("lvl 2 :" + sw.ElapsedMilliseconds);
                        Console.WriteLine(driver.GetImplicitWait());
                    }
                };
                Console.WriteLine(driver.GetImplicitWait());
                {
                    var sw = Stopwatch.StartNew();
                    //var exist = driver.Get("sadlkjaslkjd").SetSearchElementTimeout(20).Exist;
                    sw.Stop();
                    Console.WriteLine("lvl 1 :" + sw.ElapsedMilliseconds);
                    Console.WriteLine(driver.GetImplicitWait());
                }
            };
            Console.WriteLine(driver.GetImplicitWait());


            var elements = driver.Get("/*").GetAll(".//*");
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
}
