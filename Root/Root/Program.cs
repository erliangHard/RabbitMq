﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Root_Extensions;
using Root_Consume.Services;

namespace Root
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
                    services.AddHostedService<MsgFailSuccessTestServerA>();
                    services.AddHostedService<MsgFailSuccessTestServerB>();
                }).ConfigureLogging((hostContext, configLog) =>
                {
                    configLog.AddConsole();
                }).Build();

            host.Run();

        }
    }
}
