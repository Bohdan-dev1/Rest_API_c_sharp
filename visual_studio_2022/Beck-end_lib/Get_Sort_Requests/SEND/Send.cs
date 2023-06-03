using System.Net;
using System.Text;

namespace Beck_end_lib.Get_Sort_Requests.SEND
{
    internal class Send
    {
        public static async void SendData(HttpListenerResponse resp, string data) {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            resp.ContentLength64 = buffer.Length;

            using Stream ros = resp.OutputStream;
            ros.Write(buffer, 0, buffer.Length);
        }
    }
}
