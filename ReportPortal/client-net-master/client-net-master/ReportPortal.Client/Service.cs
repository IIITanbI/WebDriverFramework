using System;
using System.Collections.Generic;
using System.Net;
using ReportPortal.Client.Extentions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace ReportPortal.Client
{
    public class DriverObjectProxy : RealProxy
    {
        private MyClient client;

        public DriverObjectProxy(HttpMessageHandler handler) : base(typeof(IMyClient))
        {
            client = new MyClient(handler);
            this.Handler = handler;
        }
        public DriverObjectProxy(HttpMessageHandler handler, bool disposeHandler) : base(typeof(IMyClient))
        {
            client = new MyClient(handler, disposeHandler);
            this.Handler = handler;
            this.DisposeHandler = disposeHandler;
        }

        public HttpMessageHandler Handler { get; set; }

        public bool DisposeHandler { get; set; }


        protected static ReturnMessage InvokeMethod(object representedValue, IMethodCallMessage msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException(nameof(msg), "The message containing invocation information cannot be null");
            }

            MethodInfo proxiedMethod = msg.MethodBase as MethodInfo;
            var result = proxiedMethod.Invoke(representedValue, msg.Args);
            return new ReturnMessage(result, null, 0, msg.LogicalCallContext, msg);
        }

        public override IMessage Invoke(IMessage msg)
        {
            try
            {
                IMethodCallMessage methodCallMessage = msg as IMethodCallMessage;
                //Console.WriteLine(methodCallMessage.MethodName);
                return InvokeMethod(client, methodCallMessage);
            }
            catch (Exception ex)
            {
                lock (Console.Out)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ResetColor();
                }

                throw;
            }
        }
    }

    public class MyClient : HttpClient, IMyClient
    {
        public MyClient(HttpMessageHandler handler) : base(handler)
        {
        }

        public MyClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {
        }
    }
    /// <summary>
    /// Class to interact with common Report Portal services. Provides possibility to manage almost of service's entities.
    /// </summary>
    public partial class Service
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClientHandler _httpHandler;

        /// <summary>
        /// Constructor to initialize a new object of service.
        /// </summary>
        /// <param name="uri">Base URI for REST service.</param>
        /// <param name="project">A project to manage.</param>
        /// <param name="password">A password for user. Can be UID given from user's profile page.</param>
        public Service(Uri uri, string project, string password)
        {
            _httpHandler = new HttpClientHandler();

            //_httpClient = (IMyClient)(new DriverObjectProxy(_httpHandler).GetTransparentProxy());
            _httpClient = new HttpClient(_httpHandler);
            _httpClient.BaseAddress = uri;
            //_httpClient.Timeout = TimeSpan.FromMinutes(10);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + password);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Reporter");
            BaseUri = uri;
            Project = project;
        }

        /// <summary>
        /// Constructor to initialize a new object of service.
        /// </summary>
        /// <param name="uri">Base URI for REST service.</param>
        /// <param name="project">A project to manage.</param>
        /// <param name="password">A password for user. Can be UID given from user's profile page.</param>
        /// <param name="proxy">Proxy for all HTTP requests.</param>
        public Service(Uri uri, string project, string password, IWebProxy proxy)
            : this(uri, project, password)
        {
            _httpHandler.Proxy = proxy;
        }

        /// <summary>
        /// Get or set project name to interact with.
        /// </summary>
        public string Project { get; set; }

        public Uri BaseUri { get; set; }
    }
}
