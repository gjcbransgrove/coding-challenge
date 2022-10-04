using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;

namespace Loup.DotNet.Challenge.TestFramework
{
    public class InMemoryFunctionTestHost<TStartup> : IDisposable,
                                                      IApiTestHost
                                                      where TStartup : FunctionsStartup, IStartupModule, new()
    {
        private TStartup _startup;
        private List<ApiEndpoint> _functionDefinitions;
        private string _environment;
        private Dictionary<string, string> _dataSourceEntities { get; set; }
        public IReadOnlyDictionary<string, string> Entities => _dataSourceEntities;
        public IHost Host { get; private set; }
        public IRepository Repository {get;set;}

        public InMemoryFunctionTestHost()
        {
            _functionDefinitions = new List<ApiEndpoint>();
            _dataSourceEntities = new Dictionary<string, string>();
            _environment = "Staging";
            Repository = new Mock<IRepository>().Object;
        }

        /// <summary>
        /// <see cref="IApiTestHost.Start(string)"/>
        /// </summary>
        public void Start(string applicationRootPath)
        {
            var testAssembly = GetType().Assembly;

            var startupAssembly = typeof(TStartup).Assembly;
            if (applicationRootPath.EndsWith(".dll"))
            {
                var startupAssemblyLocation = startupAssembly.Location;
                var applicationDirectory = new DirectoryInfo(applicationRootPath);
                applicationRootPath = applicationDirectory.Parent.FullName;
            }

            _startup = new TStartup();
            _startup.ApplicationRootPath = applicationRootPath;

            var builder = new HostBuilder();
            builder.UseEnvironment(_environment);
            builder.ConfigureWebJobs(_startup.Configure);

            builder.ConfigureWebJobs(config =>
            {
                var mockHostingEnvironment = new Mock<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
                mockHostingEnvironment.Setup(m => m.EnvironmentName).Returns(_environment);
                config.Services.TryAddSingleton<Microsoft.AspNetCore.Hosting.IHostingEnvironment>(mockHostingEnvironment.Object);

                config.Services.Replace(ServiceDescriptor.Singleton(typeof(IHttpContextAccessor), f =>
                {
                    return new Mock<IHttpContextAccessor>().Object;
                }));

                
                config.Services.Replace(ServiceDescriptor.Singleton(typeof(IRepository), Repository));
                config.Services.Replace(ServiceDescriptor.Scoped(typeof(UserContext), f =>
                {
                    return new UserContext();
                }));
            });

            Host = builder.Build();

            GenerateEndpoints(startupAssembly);

            LoadDataSourceEntities(testAssembly);
        }

        private void GenerateEndpoints(Assembly startupAssembly)
        {
            IList<MethodInfo> methods = null;
            try
            {
                methods = startupAssembly.GetTypes()
                                         .SelectMany(m => m.GetMethods())
                                         .Where(m => m.GetCustomAttribute<FunctionNameAttribute>() != null)
                                         .Where(m => m.GetParameters().FirstOrDefault(p => p.GetCustomAttribute<HttpTriggerAttribute>() != null) != null)
                                         .ToList();
            }
            catch (Exception)
            {
                return;
            }

            // register functions

            foreach (var method in methods)
            {
                var trigger = method.GetParameters()
                                    .FirstOrDefault(p => p.GetCustomAttribute<HttpTriggerAttribute>() != null)
                                    .GetCustomAttribute<HttpTriggerAttribute>();

                var httpRoute = HttpUtility.UrlDecode(trigger?.Route);

                var endpoint = new ApiEndpoint(httpRoute)
                {
                    Name = method.Name,
                    ClassType = method.DeclaringType,
                    MethodInfo = method,
                    HttpMethod = new HttpMethod(trigger.Methods.First())
                };

                RegisterEndpoint(endpoint);
            }
        }

        private void LoadDataSourceEntities(Assembly testAssembly)
        {
            var resources = testAssembly.GetManifestResourceNames().Where(x => x.Contains("_response")).ToList();
            foreach (var resource in resources)
            {
                using (Stream stream = testAssembly.GetManifestResourceStream(resource))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    var jObject = JObject.Parse(result);

                    string[] keys = jObject.Value<JArray>("keys").ToObject<string[]>();
                    string value = null;
                    try
                    {
                        value = jObject.Value<JObject>("value")?.ToString();
                    }
                    catch (Exception)
                    {
                        // Fallback to try and handle array data (e.g azure search result)
                        value = jObject.Value<JArray>("value")?.ToString();
                        // handle / skip
                    }
                    foreach (var key in keys)
                        _dataSourceEntities.Add(key, value);
                }
            }
        }

        public HttpClient CreateClient()
        {
            return new HttpClient(new InMemoryHttpMessageHandler(this, Host.Services));
        }

        public string GetResponse(string responseDataKey)
        {
            var entity = _dataSourceEntities.SingleOrDefault(e => e.Key == responseDataKey);
            return entity.Value;
        }

        public (ApiEndpoint Endpoint, IDictionary<string, object> Parameters) ResolveEndpoint(HttpMethod method, string path)
        {
            foreach (var function in _functionDefinitions)
            {
                if (function.Matches(method, path, out IDictionary<string, object> parameters))
                    return (function, parameters);
            }

            return (null, null);
        }

        public void RegisterEndpoint(ApiEndpoint endpoint)
        {
            _functionDefinitions.Add(endpoint);
        }

        public void Dispose()
        {
            Host.Dispose();
        }
    }
}
