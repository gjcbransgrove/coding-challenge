using Loup.DotNet.Challenge.TestFramework;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Loup.DotNet.Challenge.Tests.TestFramework
{
    public class ApiEndpointTests
    {
        [Theory]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        public void When_ApiEndpoint_Uses_Different_HttpMethod_Expect_No_Match(string method)
        {
            var apiEndpoint = new ApiEndpoint("test/endpoint")
            {
                Name = "Test",
                ClassType = new Mock<Type>().Object,
                MethodInfo = new Mock<MethodInfo>().Object,
                HttpMethod = HttpMethod.Get
            };

            var result = apiEndpoint.Matches(new HttpMethod(method), "/api/test/endpoint", out var _);

            Assert.False(result);
        }

        [Theory]
        [InlineData("GET", "test/endpoint", "/api/test/endpoint")]
        [InlineData("POST", "endpoint/test", "/api/endpoint/test")]
        public void When_ApiEndpoint_Segments_Unparametised_And_HttpMethod_Are_Same_Expect_Match(string method, string route, string path)
        {
            var apiEndpoint = new ApiEndpoint(route)
            {
                Name = "Test",
                ClassType = new Mock<Type>().Object,
                MethodInfo = new Mock<MethodInfo>().Object,
                HttpMethod = new HttpMethod(method)
            };

            var result = apiEndpoint.Matches(new HttpMethod(method), path, out var _);

            Assert.True(result);
        }

        [Theory]
        [InlineData("POST", "test/endpoint", "/test/endpoint")]
        [InlineData("GET", "endpoint/test", "/endpoint/test")]
        public void When_ApiEndpoint_Does_Not_Use_useApiPrefix_And_Not_In_Path_Expect_Match(string method, string route, string path)
        {
            var apiEndpoint = new ApiEndpoint(route, false)
            {
                Name = "Test",
                ClassType = new Mock<Type>().Object,
                MethodInfo = new Mock<MethodInfo>().Object,
                HttpMethod = new HttpMethod(method)
            };

            var result = apiEndpoint.Matches(new HttpMethod(method), path, out var _);

            Assert.True(result);
        }

        [Theory]
        [InlineData("one/two/three", "/api/one/three/two")]
        [InlineData("three/two/one", "/api/three/two/zero")]
        [InlineData("four/five/six", "/api/six/five/four")]
        public void When_ApiEndpoint_Route_Differs_From_Path_Expect_No_Match(string route, string path)
        {
            var apiEndpoint = new ApiEndpoint(route, false)
            {
                Name = "Test",
                ClassType = new Mock<Type>().Object,
                MethodInfo = new Mock<MethodInfo>().Object,
                HttpMethod = HttpMethod.Get
            };

            var result = apiEndpoint.Matches(HttpMethod.Get, path, out var _);

            Assert.False(result);
        }

        [Fact]
        public void When_ApiEndpoint_Route_Contains_Parameters_Expect_Parameters_Out_To_Contain_Values()
        {
            var methodInfoMock = new Mock<MethodInfo>();
            var paramInfoMock = new Mock<ParameterInfo>();
            paramInfoMock.SetupGet(x => x.Name).Returns("parameterId");
            paramInfoMock.SetupGet(x => x.ParameterType).Returns(typeof(int));
            methodInfoMock.Setup(x => x.GetParameters()).Returns(new ParameterInfo[] { paramInfoMock.Object });

            var apiEndpoint = new ApiEndpoint("route/with/{parameterId}")
            {
                Name = "Test",
                ClassType = new Mock<Type>().Object,
                MethodInfo = methodInfoMock.Object,
                HttpMethod = HttpMethod.Get
            };

            var result = apiEndpoint.Matches(HttpMethod.Get, "/api/route/with/123", out var parameters);

            Assert.True(result);
            Assert.True(parameters.ContainsKey("parameterId"));
            Assert.Equal(123, parameters["parameterId"]);
        }

        [Fact]
        public void When_ApiEndpoint_BuildParameters_Recieves_Valid_Parameters_Expect_Complete_Parameter_Dictionary()
        {
            var methodInfoMock = new Mock<MethodInfo>();

            var paramInfoMock = new Mock<ParameterInfo>();
            paramInfoMock.SetupGet(x => x.Name).Returns("parameterId");
            paramInfoMock.SetupGet(x => x.ParameterType).Returns(typeof(int));

            var secondParameterMock = new Mock<ParameterInfo>();
            secondParameterMock.SetupGet(x => x.Name).Returns("secondParameter");
            secondParameterMock.SetupGet(x => x.ParameterType).Returns(typeof(string));

            methodInfoMock
                .Setup(x => x.GetParameters())
                .Returns(new ParameterInfo[] { paramInfoMock.Object, secondParameterMock.Object });

            var apiEndpoint = new ApiEndpoint("route/with/{parameterId}/{secondParameter}")
            {
                Name = "Test",
                ClassType = new Mock<Type>().Object,
                MethodInfo = methodInfoMock.Object,
                HttpMethod = HttpMethod.Get
            };

            var parameterValueList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("{parameterId}", "123"),
                new KeyValuePair<string, string>("{secondParameter}", "asdf")
            };

            var result = apiEndpoint.BuildParameters(parameterValueList);

            Assert.True(result.ContainsKey("parameterId"));
            Assert.True(result.ContainsKey("secondParameter"));
            Assert.Equal(123, result["parameterId"]);
            Assert.Equal("asdf", result["secondParameter"]);
        }
    }
}
