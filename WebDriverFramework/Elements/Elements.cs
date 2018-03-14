namespace WebDriverFramework.Elements
{
    using OpenQA.Selenium;
    using System.Threading;
    using System.Windows.Forms;

    public class FrameElement : WebElement, IFrameElement, ILocate<FrameElement>
    {
        public FrameElement(IWebElement implicitElement, WebDriver driver) : base(implicitElement, driver)
        {
        }
        public FrameElement(By locator, WebElement parent, WebDriver driver = null) : base(locator, parent, driver)
        {
        }

        public FrameElement Locate() => new FrameElement(this.Element, this.Driver);
    }

    public class LabelElement : WebElement, ILocate<LabelElement>
    {
        public LabelElement(IWebElement implicitElement, WebDriver driver) : base(implicitElement, driver)
        {
        }
        public LabelElement(By locator, WebElement parent, WebDriver driver = null) : base(locator, parent, driver)
        {
        }

        public string Text => Element.Text;

        public LabelElement Locate() => new LabelElement(this.Element, this.Driver);
    }

    public abstract class InputWebElement : WebElement
    {
        protected InputWebElement(IWebElement implicitElement, WebDriver driver) : base(implicitElement, driver)
        {
        }
        protected InputWebElement(By locator, WebElement parent, WebDriver driver = null) : base(locator, parent, driver)
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

        public TextWebElement(IWebElement implicitElement, WebDriver driver) : base(implicitElement, driver)
        {
        }
        public TextWebElement(By locator, WebElement parent, WebDriver driver = null) : base(locator, parent, driver)
        {
        }

        public bool Enabled => Element.Enabled;

        public void Clear(ILogger log = null)
        {
            this.Element.Clear();
        }
        public void Submit(ILogger log = null)
        {
            this.Element.Submit();
        }
        public void CopyPaste(string text, ILogger log = null)
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

        public void Send(string text, ILogger log = null)
        {
            this.Element.SendKeys(text);
        }
        public void SendByChars(string text, ILogger log = null)
        {
            foreach (var ch in text)
            {
                this.Send(ch.ToString());
            }
        }

        public void SelectText(ILogger log = null)
        {
            this.Send(OpenQA.Selenium.Keys.LeftControl + "a");
        }
        public void ClearAndSetText(string text, ILogger log = null)
        {
            this.Clear();
            this.Send(text);
        }
        public void ClearAndSetTextByChars(string text, ILogger log = null)
        {
            this.Clear();
            this.SendByChars(text);
        }
        public void SelectAndSetText(string text, ILogger log = null)
        {
            this.SelectText();
            this.Send(text);
        }
        public void SelectAndSetTextByChars(string text, ILogger log = null)
        {
            this.SelectText();
            this.SendByChars(text);
        }

        public TextWebElement Locate() => new TextWebElement(this.Element, this.Driver);
    }

    public class CheckBox : InputWebElement, ILocate<CheckBox>
    {
        public CheckBox(IWebElement implicitElement, WebDriver driver) : base(implicitElement, driver)
        {
        }
        public CheckBox(By locator, WebElement parent, WebDriver driver = null) : base(locator, parent, driver)
        {
        }

        public bool Selected => this.Element.Selected;

        public void Select(ILogger log = null) => SetState(true, log);
        public void Deselect(ILogger log = null) => SetState(false, log);
        public void SetState(bool state, ILogger log = null)
        {
            if (this.Element.Selected != state)
            {
                this.Element.Click();
            }
        }

        public CheckBox Locate() => new CheckBox(this.Element, this.Driver);
    }
}