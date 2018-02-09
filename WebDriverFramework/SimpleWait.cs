namespace WebDriverFramework
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using System;

    public class SimpleWait
    {
        private readonly DefaultWait<object> _wait = new DefaultWait<object>(new object(), new SystemClock());

        private static TimeSpan DefaultSleepTimeout => TimeSpan.FromMilliseconds(500);

        public SimpleWait(TimeSpan timeout) : this(timeout, DefaultSleepTimeout)
        {
        }

        public SimpleWait(TimeSpan timeout, TimeSpan sleepInterval)
        {
            this.Timeout = timeout;
            this.PollingInterval = sleepInterval;
            this.IgnoreExceptionTypes(typeof(NoSuchElementException));
        }

        public TimeSpan Timeout
        {
            get => _wait.Timeout;
            set => _wait.Timeout = value;
        }
        public TimeSpan PollingInterval
        {
            get => _wait.PollingInterval;
            set => _wait.PollingInterval = value;
        }
        public string Message
        {
            get => _wait.Message;
            set => _wait.Message = value;
        }

        public void IgnoreExceptionTypes(params Type[] exceptionTypes) => _wait.IgnoreExceptionTypes(exceptionTypes);

        public TResult Until<TResult>(Func<TResult> condition)
        {
            return _wait.Until(o => condition());
        }
    }
}