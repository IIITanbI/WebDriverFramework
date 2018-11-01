namespace WebDriverFramework
{
    using Elements;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.Extensions;
    using OpenQA.Selenium.Support.UI;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Threading;

    public class WebDriver : IGetElement, IGetElements
    {
        public WebDriver(IWebDriver nativeDriver)
        {
            this.NativeDriver = nativeDriver;
        }

        public IWebDriver NativeDriver { get; }

        public T Get<T>(By locator) => ElementFactory.Create<T>(locator, null, this);
        public IEnumerable<T> GetAll<T>(By locator) => new FindAllHelper<T>(this, locator);

        public WebDriverWait GetWait(double timeout, params Type[] exceptionTypes)
        {
            var wait = new WebDriverWait(this.NativeDriver, TimeSpan.FromSeconds(timeout));
            wait.IgnoreExceptionTypes(exceptionTypes);
            return wait;
        }
        public T Wait<T>(Func<WebDriver, T> condition, double timeout, params Type[] exceptionTypes)
        {
            return this.GetWait(timeout, exceptionTypes).Until(d => condition(this));
        }

        public void ExecuteJavaScript(string script, WebElement element, ILogger log = null)
        {
            this.ExecuteJavaScript<object>(script, element, log);
        }
        public T ExecuteJavaScript<T>(string script, WebElement element, ILogger log = null)
        {
            return this.ExecuteJavaScript<T>(script, new object[] { element.Element }, log);
        }

        public void ExecuteJavaScript(string script, object[] args, ILogger log = null)
        {
            this.ExecuteJavaScript<object>(script, args, log);
        }
        public T ExecuteJavaScript<T>(string script, object[] args, ILogger log = null)
        {
            return this.NativeDriver.ExecuteJavaScript<T>(script, args);
        }

        public void SwitchToDefaultContent(ILogger log = null)
        {
            this.NativeDriver.SwitchTo().DefaultContent();
        }
        public void SwitchToFrame(IFrameElement element, ILogger log = null)
        {
            this.NativeDriver.SwitchTo().Frame(element.Element);
        }

        public void Quit()
        {
            this.NativeDriver.Quit();
        }

        private Bitmap AddCropImage(Bitmap source, Image addedBitmap, Rectangle section)
        {
            int height = source?.Height ?? 0;
            int width = source?.Width ?? 0;
            if (width != 0 && width != section.Width)
                throw new Exception("Section width not equal to addToBitmap width");

            Bitmap bmp = new Bitmap(section.Width, section.Height + height);
            Graphics g = Graphics.FromImage(bmp);
            if (height > 0 && width > 0)
                g.DrawImage(source, 0, 0, new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);

            g.DrawImage(addedBitmap, 0, height, section, GraphicsUnit.Pixel);

            return bmp;
        }
        public Bitmap MakeScreenshot(ILogger log, bool fullScreen = true)
        {
            Func<Bitmap> getScreenshot = () =>
            {
                Screenshot screenshot = ((ITakesScreenshot)this.NativeDriver).GetScreenshot();
                Bitmap bmp = null;
                using (var ms = new MemoryStream(screenshot.AsByteArray))
                    bmp = new Bitmap(ms);
                return bmp;
            };

            if (!fullScreen)
            {
                return getScreenshot();
            }

            Bitmap result = null;

            long clientHeight = ExecuteJavaScript<long>("return window.innerHeight", new object[0]);
            long clientWidth = ExecuteJavaScript<long>("return document.body.clientWidth", new object[0]);
            long scrollHeight = ExecuteJavaScript<long>("return document.body.scrollHeight", new object[0]);

            for (long cur = 0; cur < scrollHeight; cur += clientHeight)
            {
                Rectangle section;
                if (cur > scrollHeight - clientHeight)
                {
                    long height = cur - (scrollHeight - clientHeight);
                    section = new Rectangle(0, (int)height, (int)clientWidth, (int)(clientHeight - height));
                }
                else
                {
                    section = new Rectangle(0, 0, (int)clientWidth, (int)clientHeight);
                }

                ExecuteJavaScript($"document.documentElement.scrollTop={cur}", new object[0]);
                Thread.Sleep(1000);

                using (Bitmap bmp = getScreenshot())
                {
                    result = AddCropImage(result, bmp, section);
                }
            }

            return result;
        }
    }
}
