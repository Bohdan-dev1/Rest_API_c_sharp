using Beck_end_lib.Get_Sort_Requests.SEND;
using System.Net;
using System.Text.Json;

namespace Beck_end_lib.Get_Sort_Requests.Default
{
    public class Def_Req
    {
        public string? def_text { get; set; }
        public string? error { get; set; }
        public int? httpCode { get; set; }

    }
    internal class Default_Request
    {
        public void Undefined_URL( HttpListenerResponse resp, string text_error )
        {
            Def_Req def_Req = new Def_Req {
                def_text = text_error,
                error = "undefined",
                httpCode = 404,
            };
            string data = JsonSerializer.Serialize(def_Req);
            Send.SendData(resp, data);
        }
    }
}
