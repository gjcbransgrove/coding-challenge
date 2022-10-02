using Loup.DotNet.Challenge.FunctionApp.Models;
using Loup.DotNet.Challenge.TestFramework;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Loup.DotNet.Challenge.Tests
{
    /// <summary>
    /// Add your tests to this class.
    /// </summary>
    [Collection(nameof(FunctionAppTestHost))]
    public class Tests
    {
        private readonly ITestOutputHelper _helper;
        private readonly FunctionAppTestHost _testHost;

        public Tests(ITestOutputHelper helper, FunctionAppTestHost testHost)
        {
            _helper = helper;
            _testHost = testHost;
        }

        [Fact]
        public async Task When_UserContext_Is_Authenticated_False_Expect_Unauthorized_Template()
        {
            
        }

        [Fact]
        public async Task When_UserContext_Is_Authenticated_True_Expect_No_Unauthorized_Response()
        {
            
        }

        [Fact]
        public async Task When_Content_Id_Invalid_Integer_Expect_Bad_Request_Template()
        {
            
        }

        [Fact]
        public async Task When_Content_Id_In_Repository_Expect_Recipe_Template()
        {
            
        }
    }
}