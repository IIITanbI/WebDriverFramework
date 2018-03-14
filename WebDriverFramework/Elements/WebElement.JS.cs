namespace WebDriverFramework.Elements
{
    public abstract partial class WebElement
    {
        public string JsText => this.Driver.ExecuteJavaScript<string>("arguments[0].text", this);

        public void JSClick(ILogger log = null)
        {
            this.Driver.ExecuteJavaScript("arguments[0].click()", this, log);
        }
        public void JSScrollIntoView(ILogger log = null)
        {
            this.Driver.ExecuteJavaScript("arguments[0].scrollIntoView(true)", this, log);
        }
        public void JSScrollTo(ILogger log = null)
        {
            this.Driver.ExecuteJavaScript($"window.scrollTo({this.Element.Location.X}, {this.Element.Location.Y})", new object[0], log);
        }
    }
}