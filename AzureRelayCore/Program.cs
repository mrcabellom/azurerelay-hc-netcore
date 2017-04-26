using System;
using AzureRelayCore;
using AzureRelayCore.Infrastructure;

class Program
{
    static void Main(string[] args)
    {
        Startup startup = new Startup();
        IServiceProvider serviceProvider = startup.ConfigureServices();
        IStream relayConnection = (IStream)serviceProvider.GetService(typeof(IStream));
        relayConnection.StartStreamAsync().GetAwaiter().GetResult();
    }
}