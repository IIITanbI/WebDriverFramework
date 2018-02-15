using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace WebDriverFramework
{
    public class Condition
    {
        public static bool Exist(WebElement e) => e.Exist;
        public static bool NotExist(WebElement e) => !Exist(e);
        public static bool Displayed(WebElement e) => e.Exist && e.Displayed;
        public static bool NotDisplayed(WebElement e) => !Displayed(e);
    }

    public class MyCondition
    {
        public static ExistCondition Exist => new ExistCondition();
        public static NotExistCondition NotExist => new NotExistCondition();

        protected Dictionary<WebElement, int> callDictionary = new Dictionary<WebElement, int>(3);

        protected int GetCallCount(WebElement e)
        {
            if (callDictionary.TryGetValue(e, out var cnt))
            {
                return cnt;
            }

            callDictionary.Add(e, 0);
            return 0;
        }
        protected int IncrementCallCount(WebElement e)
        {
            return ++callDictionary[e];
        }

        protected bool Action(WebElement e, Func<int, bool> action)
        {
            var callCnt = GetCallCount(e);
            try
            {
                return action(callCnt);
            }
            finally
            {
                IncrementCallCount(e);
            }
        }
    }
    public class ExistCondition : MyCondition
    {
        public bool Invoke(WebElement e)
        {
            return Action(e, callCnt =>
            {
                try
                {
                    e.StubActionOnElement();
                    return true;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
                catch (StaleElementReferenceException) when (callCnt == 0 || !e.IsCached)
                {
                    return false;
                }
            });
        }
    }
    public class NotExistCondition : MyCondition
    {
        public bool Invoke(WebElement e)
        {
            try
            {
                e.StubActionOnElement();
                return true;
            }
            catch (NoSuchElementException)
            {
                return true;
            }
            catch (StaleElementReferenceException)
            {
                return true;
            }
        }
    }
}