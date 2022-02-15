using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Loup.DotNet.Challenge.FunctionApp;
using Loup.DotNet.Challenge.TestFramework;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Loup.DotNet.Challenge.FunctionApp
{
    public class Startup : FunctionsStartup, IStartupModule
    {
        private IConfigurationRoot _config;

        public string ApplicationRootPath { get; set; }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (string.IsNullOrEmpty(ApplicationRootPath))
            {
                var localRootPath = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot");
                var azureRootPath = $"{Environment.GetEnvironmentVariable("HOME")}/site/wwwroot";
                ApplicationRootPath = localRootPath ?? azureRootPath;
            }

            var configBuilder = new ConfigurationBuilder().SetBasePath(ApplicationRootPath)
                                                          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
            _config = configBuilder.Build();

            Load(builder.Services);
        }

        public void Load(IServiceCollection services)
        {
            
        }
    }
}
