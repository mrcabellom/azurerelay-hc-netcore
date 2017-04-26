using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using AzureRelayCore.Infrastructure.Services.TwitterStream;
using AzureRelayCore.Infrastructure.Services.ConsoleStream;
using AzureRelayCore.Infrastructure.Services.RelayConnection;
using AzureRelayCore.Infrastructure;

namespace AzureRelayCore
{
    public class Startup
    {
        IConfigurationRoot Configuration { get; }
        public IServiceCollection Services { get; private set; }
        public Startup()
        {
            Services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            builder.AddUserSecrets<Startup>();
            Configuration = builder.Build();
        }

        public IServiceProvider ConfigureServices()
        {
            Services.AddOptions();

            Services.Configure<TwitterConfig>(Configuration.GetSection("TwitterConfig:TwitterTokens"));
            Services.Configure<TwitterFilterOptions>(Configuration.GetSection("TwitterConfig:TwitterFilters"));
            Services.Configure<RelayConfig>(Configuration.GetSection("HybridConnection"));

            if (Convert.ToBoolean(Configuration["UseTwitterStream"]))
            {
                Services.AddTransient<IStream, TwitterStream>();
            }
            else
            {
                Services.AddTransient<IStream, ConsoleStream>();
            }

            Services.AddSingleton<IRelayConnection, RelayConnection>();
            return Services.BuildServiceProvider();
        }
    }
}
