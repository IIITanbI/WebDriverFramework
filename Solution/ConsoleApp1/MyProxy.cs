using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using WebDriverFramework;

namespace ConsoleApp1
{
    public class ProxyStub : MarshalByRefObject
    {
    }

    public class MyProxy<T> : RealProxy, IRemotingTypeInfo where T : ILog
    {
        ILogger _log = null;
        public MyProxy(T obj, ILogger log) : base(typeof(ProxyStub))
        {
            this.Object = obj;
            this._log = log;
        }

        public T Object { get; set; }

        public string TypeName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override object GetTransparentProxy()
        {
            var res = base.GetTransparentProxy();
            return res;
        }

        public bool CanCastTo(Type fromType, object o)
        {
            return fromType.IsInstanceOfType(this.Object);
        }

        public override IMessage Invoke(IMessage msg)
        {
            var originalLog = this.Object.Log;

            try
            {
                this.Object.Log = this._log;
                var methodCallMessage = (IMethodCallMessage)msg;
                var proxiedMethod = (MethodInfo)methodCallMessage.MethodBase;
                return new ReturnMessage(proxiedMethod.Invoke(this.Object, methodCallMessage.Args), null, 0, methodCallMessage.LogicalCallContext, methodCallMessage);
            }
            finally
            {
                this.Object.Log = originalLog;
            }
        }
    }
}