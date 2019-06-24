using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MQTT
{
    class GateWay
    {
        public Data data { get; set; }

        public class Data
        {
            public float temperature { get; set; }
            public float humidity { get; set; }
            public int pressure { get; set; }
        }

        public Status status { get; set; }

        public class Status
        {
            public string devEUI { get; set; }
            public int rssi { get; set; }
            public float temperature { get; set; }
            public int battery { get; set; }
            public DateTime date { get; set; }
        }
    }

    class RoomStatus
    {
        public long Id { get; set; }
        public int RoomNumber { get; set; }
        public string UserId { get; set; }
        public float Temperature { get; set; }
        public float AirHumidity { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    class BoilerStatus
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public float HeatCarrierTemperature { get; set; }
        public bool LeakOfGasStatus { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    class Listener
    {
        static void Main(string[] args)
        {
            Listen();

            Console.ReadLine();
        }

        public static void Listen()
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId("svinoludi")
                .WithTcpServer("192.168.43.253", 1883)
                .WithCleanSession()
                .Build();

            client.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine();
                Console.WriteLine("MESSAGE RECEIVED");

                Console.WriteLine("BEGIN DESERIALIZING");

                var data = JsonConvert.DeserializeObject<GateWay>(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));

                Console.WriteLine("DESERIALIZING COMPLETE");

                RoomStatus room = new RoomStatus();

                switch (data.status.devEUI)
                {
                    case "807b859020000613": room.RoomNumber = 1; break;
                    case "807b859020000001": room.RoomNumber = 2; break;
                }

                room.AirHumidity = data.data.humidity;
                room.Temperature = data.data.temperature;
                room.UpdateTime = data.status.date;
                room.UserId = "default";

                var json = JsonConvert.SerializeObject(room);

                SendJSON(json);

                BoilerToJSON();
            });

            client.UseConnectedHandler(e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                client.SubscribeAsync(new TopicFilterBuilder().WithTopic("#").Build());

                Console.WriteLine("### SUBSCRIBED ###");
            });

            client.ConnectAsync(options);
        }

        static async void SendJSON(string json)
        {
            try
            {
                var sender = new Sender();
                await sender.SendRoom(json);
            }
            catch
            {
                Console.WriteLine("Данные не были отправлены!");
            }
        }

        static async void SendBoilerJSON(string json)
        {
            try
            {
                var sender = new Sender();
                await sender.SendBoiler(json);
            }
            catch
            {
                Console.WriteLine("Данные не были отправлены!");
            }
        }

        static void BoilerToJSON()
        {
            BoilerStatus boilerStatus = new BoilerStatus();

            boilerStatus.UserId = "default";
            boilerStatus.LeakOfGasStatus = false;
            boilerStatus.UpdateTime = DateTime.Now;
            boilerStatus.HeatCarrierTemperature = (float)150;

            string boilerJson = JsonConvert.SerializeObject(boilerStatus);

            SendBoilerJSON(boilerJson);
        }
    }
}