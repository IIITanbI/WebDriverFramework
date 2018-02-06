using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.PageObjects;
using WebDriverFramework;
using WebDriverFramework.PageFactory;
using WebDriverFramework.PageFactory.Attributes;

namespace ConsoleApp1
{
    class Program
    {
        private WebDriver Driver;

        //[CacheLookup]
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
            CustomPageFactory.InitElements(driver, this, new CustomPageObjectMemberDecorator(driver));
        }

        static void Main(string[] args)
        {
            var driver = new WebDriver(new ChromeDriver());
            driver.Navigate().GoToUrl("file:///C:/Users/Artsiom_Kuis/Desktop/test.html");

            //driver.WaitForElement(TimeSpan.FromSeconds(10), By.XPath(".//test"));
            var pr = new Program(driver);

            //driver.WaitForElement(TimeSpan.FromSeconds(5), pr.element2);
            //var el = pr.element2.Element.WaitForElementDisplayed(TimeSpan.MaxValue);

            pr.element1 = new WebElement(By.XPath("//div[@class='test']"), driver);
            var element = pr.element1.WaitForElementNotDisplayed(TimeSpan.FromSeconds(10));
            var vis = pr.element2.Text;
            var vis1 = pr.element1.Text;
            var vis3 = pr.element3.Text;
            var vis4 = pr.element4.ToList();
            driver.Quit();
        }
    }
}
