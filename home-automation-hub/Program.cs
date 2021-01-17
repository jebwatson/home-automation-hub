// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Watson Laboratory">
//   Copyright © 2021 Watson Laboratory
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace home_automation_hub
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;

    internal class Program
    {
        private const string AppsettingsFilename = "appsettings.json";

        private static void HubitatAutomationEvent(object sender, AutomationEventEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now} Automation event processed!");
        }

        private static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                                          .SetBasePath(Directory.GetCurrentDirectory())
                                          .AddJsonFile(AppsettingsFilename, optional: false, reloadOnChange: true)
                                          .Build();

            var hubitat = new Hubitat(configuration);
            hubitat.AutomationEvent += HubitatAutomationEvent;

            await hubitat.StartAutomationEventWatcher();

            //// Create websocket and print device list.
            //var uri = new Uri("ws://192.168.1.115/eventsocket/devices");

            //using var ws = new ClientWebSocket();
            //var token = new CancellationTokenSource().Token;
            //var array = new byte[7000];
            //var buffer = new ArraySegment<byte>(array);
            //var running = true;

            //await ws.ConnectAsync(uri, token);
            //await ws.SendAsync(buffer, WebSocketMessageType.Binary, false, token);

            //while (running)
            //{
            //    await ws.ReceiveAsync(buffer, token);
            //    var message = Encoding.Default.GetString(buffer);

            //    Console.WriteLine(message);
            //    var input = Console.ReadLine();

            //    running = input != "end";
            //}
        }
    }
}