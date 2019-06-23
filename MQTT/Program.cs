using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MQTT
{
    class Listener
    {
        static void Main(string[] args)
        {
            Listen().Wait();

            Console.ReadLine();
        }

        public static async Task Listen()
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId("svinoludi")
                .WithTcpServer("192.168.0.100", 1883)
                .WithCleanSession()
                .Build();

            client.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"Content-Type = {e.ApplicationMessage.ContentType}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
                Console.WriteLine("MESSAGE RECEIVED");
            });

            client.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic("#").Build());

                Console.WriteLine("### SUBSCRIBED ###");
            });

            await client.ConnectAsync(options);
        }
    }
}