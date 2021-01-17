// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hubitat.cs" company="Watson Laboratory">
//   Copyright © 2021 Watson Laboratory
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace home_automation_hub
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class Hubitat
    {
        private readonly string accessToken;
        private readonly string baseMakerApiAddress;
        private readonly string webSocketUrl;

        public Hubitat(IConfiguration configuration)
        {
            HubitatOptions hubitatOptions = configuration.GetSection("Hubitat").Get<HubitatOptions>();
            this.baseMakerApiAddress = $"https://{hubitatOptions.HubitatIp}/apps/api/{hubitatOptions.MakerApiAppId}/devices";
            this.accessToken = hubitatOptions.AccessToken;
            this.webSocketUrl = $"wss://{hubitatOptions.HubitatIp}/eventsocket";
        }

        public event EventHandler<AutomationEventEventArgs> AutomationEvent;

        public Task StartAutomationEventWatcher() => this.HubitatEventWatcherThread();

        private async Task HubitatEventWatcherThread()
        {
            int wseRetryCount = 0;

            while (true)
            {
                try
                {
                    Console.WriteLine($"{DateTime.Now} Connecting to Hubitat...");

                    using (var client = new ClientWebSocket())
                    {
                        client.Options.RemoteCertificateValidationCallback += (a, b, c, d) => true;
                        await client.ConnectAsync(new Uri(this.webSocketUrl), CancellationToken.None);
                        Console.WriteLine($"{DateTime.Now} Websocket success! Watching for events.");
                        wseRetryCount = 0;
                        ArraySegment<byte> buffer;

                        while (client.State == WebSocketState.Open)
                        {
                            buffer = new ArraySegment<byte>(new byte[1024 * 4]);
                            WebSocketReceiveResult reply = await client.ReceiveAsync(buffer, CancellationToken.None);
                            string json = Encoding.Default.GetString(buffer.ToArray()).TrimEnd('\0');
                            HubEvent hubEvent = JsonConvert.DeserializeObject<HubEvent>(json);
                            this.OnAutomationEvent(new AutomationEventEventArgs { HubEvent = hubEvent });
                        }
                    }
                }
                catch (WebSocketException e)
                {
                    wseRetryCount++;
                    int waitTimeInSecs = wseRetryCount > 5 ? 150 : 5;
                    Console.WriteLine($"{DateTime.Now} Hubitat websocket error! {e.Message} -- Retrying in {waitTimeInSecs} seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(waitTimeInSecs));
                }
                catch (UriFormatException e)
                {
                    Console.WriteLine($"{DateTime.Now} URI Format Exception! Fix your config dumbass! {e.Message}");
                    await Task.Delay(Timeout.InfiniteTimeSpan);
                }
                catch (Exception e)
                {
                    // Don't care what went wrong, write it down and keep trying
                    Console.WriteLine($"{DateTime.Now} {e} {e.Message}");
                }
            }
        }

        private void OnAutomationEvent(AutomationEventEventArgs automationEventEventArgs)
        {
            this.AutomationEvent?.Invoke(this, automationEventEventArgs);
        }
    }
}