using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.PageObjects;
using WebDriverFramework;
using WebDriverFramework.Extension;
using WebDriverFramework.PageFactory;
using WebDriverFramework.PageFactory.Attributes;

namespace ConsoleApp1
{
    class Program
    {
        private WebDriver Driver;

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
            this.Driver = driver;
            CustomPageFactory.InitElements(this, driver, new CustomPageObjectMemberDecorator(driver.Driver));
        }

        static void Main(string[] args)
        {

            //IWebDriver _driver = new FirefoxDriver(new FirefoxOptions(){BrowserExecutableLocation = "C:\\Program Files\\Mozilla Firefox\\firefox.exe" });
            //IWebDriver _driver = new InternetExplorerDriver(new InternetExplorerOptions()
            //{
            //    IntroduceInstabilityByIgnoringProtectedModeSettings = true,
            //    IgnoreZoomLevel = true
            //});
            IWebDriver _driver = new ChromeDriver();
            var driver = new WebDriver(_driver);
            driver.Navigate().GoToUrl("file:///C:/Users/Artsiom_Kuis/Desktop/test.html");
            driver.RegisterDriver(0.5);
            driver.SetImplicitWait(10);
            //driver.WaitForPresent(TimeSpan.FromSeconds(10), By.XPath(".//test"));
            var pr = new Program(driver);

            Console.WriteLine(driver.GetImplicitWait());
            driver.DoWithImplicitWait(() =>
            {
                {
                    var sw = Stopwatch.StartNew();
                    var exist = driver.Get("sadlkjaslkjd").Exist;
                    sw.Stop();
                    Console.WriteLine("lvl 1 :" + sw.ElapsedMilliseconds);
                }
                Console.WriteLine(driver.GetImplicitWait());
                driver.DoWithImplicitWait(() =>
                {
                    {
                        var sw = Stopwatch.StartNew();
                        var exist = driver.Get("sadlkjaslkjd").Exist;
                        sw.Stop();
                        Console.WriteLine("lvl 2 :" + sw.ElapsedMilliseconds);
                        Console.WriteLine(driver.GetImplicitWait());
                    }
                }, 5);
                Console.WriteLine(driver.GetImplicitWait());
                {
                    var sw = Stopwatch.StartNew();
                    var exist = driver.Get("sadlkjaslkjd").Exist;
                    sw.Stop();
                    Console.WriteLine("lvl 1 :" + sw.ElapsedMilliseconds);
                    Console.WriteLine(driver.GetImplicitWait());
                }
            }, 20);
            Console.WriteLine(driver.GetImplicitWait());
            var elements = driver.Get("/*").GetAll(".//*");
            var zzzxczx = elements.Count;
            var test = elements.Locate();
            test.CheckStaleness();
            test.Locate();
            var test1 = elements.ToList();
            var rrrrr = pr.element1.Get(".").Locate();
            if (driver.WaitForElement(By.XPath("some"), 10).Get("myxpath").Exist)
            {

            }

            if (driver.Get("some").WaitForPresent(10).Get("myxpath").Exist is var el)
            {
            }

            var rr2 = new WebElement(By.XPath("some"), driver.Driver);
            var rr3 = driver.WaitForElement(By.XPath("some"), 10);
            var rr4 = new WebElement(By.XPath("some"), _driver).WaitForPresent(10);
            var rr5 = driver.Get(By.XPath("some")).WaitForPresent(10);
            //driver.WaitForPresent(TimeSpan.FromSeconds(5), pr.element2);
            //var el = pr.element2.Element.WaitForElementDisplayed(TimeSpan.MaxValue);
            ListWebElement l = new ListWebElement(By.XPath("wqe"), _driver);


            var elem = pr.element1.Get("asd").WaitForPresent(10).Get(".//test").Text;
            var result = pr.element1.Get(".//test").Locate();
            var ggg = pr.element1.Displayed;
            var gg = pr.element1.WaitForElementDisplayed(10);
            var res1 = driver.FindElement(By.XPath(".//*")).Displayed;
            pr.element1 = new WebElement(By.XPath("//div[@class='test']"), driver.Driver);
            var vis = pr.element2.Text;
            var vis3 = pr.element3.Text;
            var vis4 = pr.element4.ToList();
            driver.Quit();
        }
    }
}
