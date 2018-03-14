namespace WebDriverFramework.Elements
{
    public interface ILocate<out T>
    {
        T Locate();
    }
}