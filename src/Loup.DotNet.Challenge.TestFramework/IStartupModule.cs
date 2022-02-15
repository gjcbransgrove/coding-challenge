using Microsoft.Extensions.DependencyInjection;

namespace Loup.DotNet.Challenge.TestFramework
{
    public interface IStartupModule
    {
        public string ApplicationRootPath { get; set; }

        void Load(IServiceCollection services);
    }
}
