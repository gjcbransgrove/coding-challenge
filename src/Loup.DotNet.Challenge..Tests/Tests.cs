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
    }
}