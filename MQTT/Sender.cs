using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MQTT
{
    class Sender
    {
        public async Task SendRoom(string json)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri("http://192.168.43.253:5000/api/RoomStatus");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PostAsync(uri, content);

            Console.WriteLine("Данные отправлены!");
        }
        public async Task SendBoiler(string json)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri("http://192.168.43.253:5000/api/BoilerStatus");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PostAsync(uri, content);

            Console.WriteLine("Данные отправлены!");
        }
    }
}