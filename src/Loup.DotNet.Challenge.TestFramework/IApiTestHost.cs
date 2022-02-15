using System.Collections.Generic;
using System.Net.Http;

namespace Loup.DotNet.Challenge.TestFramework
{
    /// <summary>
    /// The test host provides a single start method to host and bootstrap all required components in order to run tests against an API.
    /// </summary>
    public interface IApiTestHost
    {
        void Start(string applicationRootPath);
        HttpClient CreateClient();
        string GetResponse(string responseDataKey);

        (ApiEndpoint Endpoint, IDictionary<string, object> Parameters) ResolveEndpoint(HttpMethod method, string path);

        void RegisterEndpoint(ApiEndpoint endpoint);

        public IRepository Repository { get; set; }
    }
}
