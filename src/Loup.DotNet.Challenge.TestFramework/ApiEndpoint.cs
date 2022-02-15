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
            bool isMatch = true;
            parameters = new Dictionary<string, object>();
            var pathSegments = path.Split("/", StringSplitOptions.RemoveEmptyEntries);

            if (method != HttpMethod || pathSegments.Length != Segments.Length)
                return false;

            for (int i = 0; i < Segments.Length; i++)
            {
                var routeSegment = Segments[i];
                if (!routeSegment.Contains("{"))
                {
                    if (routeSegment != pathSegments[i])
                    {
                        isMatch = false;
                        break;
                    }
                    continue;
                }

                // extract param
                int paramTrimLength = routeSegment.Length - 2;
                var paramName = routeSegment.Substring(1, paramTrimLength);

                if (paramName.Contains(":"))
                    paramName = paramName.Substring(0, paramName.IndexOf(":"));

                ParameterInfo paramInfo = MethodInfo.GetParameters().FirstOrDefault(p => p.Name == paramName);
                if (paramInfo != null)
                {
                    if (paramInfo.ParameterType.UnderlyingSystemType == typeof(Int32))
                    {
                        if (int.TryParse(pathSegments[i], out int intValue))
                            parameters.Add(paramName, intValue);

                        continue;
                    }

                    parameters.Add(paramName, pathSegments[i]);
                }
            }

            return isMatch;
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
