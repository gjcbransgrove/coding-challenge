using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net.Http;

namespace Loup.DotNet.Challenge.TestFramework
{
    public class ApiEndpoint
    {
        public ApiEndpoint(string route, bool useApiPrefix = true)
        {
            if (useApiPrefix)
                Route += "/api/";

            Route += route;
            Segments = Route.Split("/", StringSplitOptions.RemoveEmptyEntries);
        }
        public string Name { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public Type ClassType { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public string Route { get; }
        public string[] Segments { get; }

        public bool Matches(HttpMethod method, string path, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();
            if (method != HttpMethod)
                return false;

            var pathSegments = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (pathSegments.Length != Segments.Length)
                return false;

            var parameterList = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < Segments.Length; i++)
            {
                var routeSegment = Segments[i];
                if (routeSegment.StartsWith("{"))
                {
                    parameterList.Add(new KeyValuePair<string, string>(routeSegment, pathSegments[i]));
                    continue;
                }

                if (routeSegment != pathSegments[i])
                {
                    return false;
                }
            }

            parameters = BuildParameters(parameterList);

            return true;
        }

        public IDictionary<string, object> BuildParameters(List<KeyValuePair<string, string>> parameterValueList)
        {
            var parameters = new Dictionary<string, object>();
            foreach (var kvp in parameterValueList)
            {
                var routeSegment = kvp.Key;
                var pathSegment = kvp.Value;
                // extract param
                int paramTrimLength = routeSegment.Length - 2;
                var paramName = routeSegment.Substring(1, paramTrimLength);

                if (paramName.Contains(":"))
                    paramName = paramName.Substring(0, paramName.IndexOf(":"));

                ParameterInfo paramInfo = MethodInfo.GetParameters().FirstOrDefault(p => p.Name == paramName);
                if (paramInfo == null)
                    continue;
                
                if (paramInfo.ParameterType.UnderlyingSystemType == typeof(Int32))
                {
                    if (int.TryParse(pathSegment, out int intValue))
                        parameters.Add(paramName, intValue);

                    continue;
                }

                parameters.Add(paramName, pathSegment);
                
            }
            return parameters;
        }

        public async Task<IActionResult> InvokeAsync(IRepository repository, object[] parameters)
        {
            var functionInstance = Activator.CreateInstance(ClassType, repository);
            MethodInfo methodInfo = ClassType.GetMethod(Name);
            
            var actionResult = await (Task<IActionResult>)methodInfo.Invoke(functionInstance, parameters.ToArray());

            return actionResult;
        }
    }
}
