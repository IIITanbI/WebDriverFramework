namespace WebDriverFramework.Elements
{
    using OpenQA.Selenium;
    using System.Threading;
    using System.Windows.Forms;

    public class LabelElement : WebElement, ILocate<LabelElement>
    {
        public LabelElement(IWebElement implicitElement, IWebDriver driver) : base(implicitElement, driver)
        {
        }
        public LabelElement(By locator, WebElement parent, IWebDriver driver = null) : base(locator, parent, driver)
        {
        }

        public string Text => Element.Text;

        public LabelElement Locate() => new LabelElement(this.Element, this.WrappedDriver);
    }

    public abstract class InputWebElement : WebElement
    {
        protected InputWebElement(IWebElement implicitElement, IWebDriver driver) : base(implicitElement, driver)
        {
        }
        protected InputWebElement(By locator, WebElement parent, IWebDriver driver = null) : base(locator, parent, driver)
        {
        }

        public string Value => GetAttribute("value");
    }

    public class TextWebElement : InputWebElement, ILocate<TextWebElement>
    {
        /// <summary>
        /// Object for locking while using Clipboard.
        /// </summary>
        private static readonly object Lock = new object();

        public TextWebElement(IWebElement implicitElement, IWebDriver driver) : base(implicitElement, driver)
        {
        }
        public TextWebElement(By locator, WebElement parent, IWebDriver driver = null) : base(locator, parent, driver)
        {
        }

        public bool Enabled => Element.Enabled;

        public void Clear()
        {
            this.Element.Clear();
        }
        public void Submit()
        {
            this.Element.Submit();
        }
        public void CopyPaste(string text)
        {
            lock (Lock)
            {
                var thread = new Thread(
                    () =>
                    {
                        this.Click();
                        this.Send(OpenQA.Selenium.Keys.LeftControl + "a");
                        Clipboard.SetText(text);
                        this.Send(OpenQA.Selenium.Keys.LeftControl + "v");
                    });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }
        }

        public void Send(string text)
        {
            this.Element.SendKeys(text);
        }
        public void SendByChars(string text)
        {
            foreach (var ch in text)
            {
                this.Send(ch.ToString());
            }
        }

        public void SelectText()
        {
            this.Send(OpenQA.Selenium.Keys.LeftControl + "a");
        }
        public void ClearAndSetText(string text)
        {
            this.Clear();
            this.Send(text);
        }
        public void ClearAndSetTextByChars(string text)
        {
            this.Clear();
            this.SendByChars(text);
        }
        public void SelectAndSetText(string text)
        {
            this.SelectText();
            this.Send(text);
        }
        public void SelectAndSetTextByChars(string text)
        {
            this.SelectText();
            this.SendByChars(text);
        }

        public TextWebElement Locate() => new TextWebElement(this.Element, this.WrappedDriver);
    }

    public class CheckBox : InputWebElement, ILocate<CheckBox>
    {
        public CheckBox(IWebElement implicitElement, IWebDriver driver) : base(implicitElement, driver)
        {
        }
        public CheckBox(By locator, WebElement parent, IWebDriver driver = null) : base(locator, parent, driver)
        {
        }

        public bool Selected => Element.Selected;
        public CheckBox Locate() => new CheckBox(this.Element, this.WrappedDriver);
    }
}