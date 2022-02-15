using Loup.DotNet.Challenge.FunctionApp;
using Loup.DotNet.Challenge.TestFramework;
using Xunit;

namespace Loup.DotNet.Challenge.Tests
{
    public class FunctionAppTestHost : InMemoryFunctionTestHost<Startup>
    {
        public FunctionAppTestHost() : base()
        {
            Start(typeof(Startup).Assembly.Location);
        }
    }

    [CollectionDefinition(nameof(FunctionAppTestHost))]
    public class TestFunctionAppTestHostCollection : ICollectionFixture<FunctionAppTestHost>
    { }
}
