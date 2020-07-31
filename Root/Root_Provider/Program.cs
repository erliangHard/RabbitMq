using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Root_Extensions;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Root_Provider.Service;

namespace Root_Provider
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder().
                ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());

                }).ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", true);
                }).ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddLogging();
                    services.AddRabbitMqConnection(hostContext.Configuration);
                    services.AddHostedService<MsgFailSuccessTestServer>();
                }).ConfigureLogging((hostContext, configLog) =>
                {
                    configLog.AddConsole();
                }).Build();

            host.Run();
        }
    }
}
