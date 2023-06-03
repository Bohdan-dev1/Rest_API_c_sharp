using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;


namespace Beck_end_lib
{
    public class WeatherForecast
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string? Summary { get; set; }
    }

    interface ISendRequst
    {
        public void sendJSON( HttpListenerResponse resp);
    }


    interface ISendProcessor
    {
        void ProcesserSend(ISendRequst data, HttpListenerResponse resp);
    }


    public class RestAPIsendProcessor : ISendProcessor
    {
        void ISendProcessor.ProcesserSend(ISendRequst data, HttpListenerResponse resp)
        {
            data.sendJSON(resp);
        }
    }

    public class SendRequst: ISendRequst
    {

            void ISendRequst.sendJSON(HttpListenerResponse resp)
            {
                var weatherForecast = new WeatherForecast
                {
                    Date = DateTime.Parse("2019-08-01"),
                    TemperatureCelsius = 25,
                    Summary = ".Net server version 6.0 - win 11 Test JSON"
                };

                string data = JsonSerializer.Serialize(weatherForecast);
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                resp.ContentLength64 = buffer.Length;

                using Stream ros = resp.OutputStream;
                ros.Write(buffer, 0, buffer.Length);
            }
        
    }
}
