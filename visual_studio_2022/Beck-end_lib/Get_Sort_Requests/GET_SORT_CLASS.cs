using System.Net;
using System.Text.Json;
using Microsoft.Data.SqlClient;

using Beck_end_lib.Get_Sort_Requests.SEND;
using Beck_end_lib.Get_Sort_Requests.GET;
using Beck_end_lib.Get_Sort_Requests.Serializes.UserGet;

namespace Beck_end_lib.Get_Sort_Requests
{

    public class ANSWER_SERIALIZE_CLASS<T>
    {
        public string? Date_answer { get; set; }
        public string? Name_answer { get; set; }
        public List<T>? Json_answer { get; set; }
    }

    interface IAnswerGET
    {
        void GET_ABOUT_SERVER(HttpListenerResponse resp);
        void GET_ALL_BOOKS(HttpListenerResponse resp, SqlConnection con);
        void GET_BOOK(HttpListenerResponse resp, SqlConnection con, string urlRequest, string param);
        void SEARCH_BY_INFO(HttpListenerResponse resp, SqlConnection con, string urlRequest);
        void GET_CLIENTS_LIST(HttpListenerResponse resp, SqlConnection con);
        //void GET_TABLE_USERS(HttpListenerResponse resp, SqlConnection con);
        void SEARCH_CLIENT(HttpListenerResponse resp, SqlConnection con, string urlRequest);
    }

    interface IAnswerProcessor 
    {
        void IAprocessor
            (IAnswerGET callFunc, 
            HttpListenerResponse resp, 
            SqlConnection connect, 
            string url,
            string rawURL);
    }

    public class GetSortDATA : IAnswerProcessor
    {
        void IAnswerProcessor.IAprocessor
            (IAnswerGET callFunc, 
            HttpListenerResponse resp, 
            SqlConnection connect, 
            string url,
            string rawURL)
        {

            switch (rawURL)
            {
                case "/GetAllBooks":
                    callFunc.GET_ALL_BOOKS(resp,connect);
                    break;

                case "/GetBookInfo_AND":
                    callFunc.GET_BOOK(resp, connect, url, "AND");
                    break;

                case "/GetBookInfo_OR":
                    callFunc.GET_BOOK(resp, connect, url, "OR");
                    break;

                case "/SearchLikeInfoBook":
                    callFunc.SEARCH_BY_INFO(resp, connect, url);
                    break;

                case "/GetAboutServer":
                    callFunc.GET_ABOUT_SERVER(resp);
                    break;

                case "/GetAllClients":
                    callFunc.GET_CLIENTS_LIST(resp,connect);
                    break;

                case "/SearchClient":
                    callFunc.SEARCH_CLIENT(resp, connect, url);
                    break;
            }
        }
    }

    internal class GET_CLASS : IAnswerGET
    {

        void IAnswerGET.GET_ABOUT_SERVER(HttpListenerResponse resp)
        {
            var answer_json = new ANSWER_SERIALIZE_CLASS<string> {
                Date_answer = DateTime.Now.ToString(),
                Name_answer = "ABOUT_SERVER",
                Json_answer = new List<string>() {{ ".Net 6.0, core - Windows NT 11" }} , 
            };

            string data = JsonSerializer.Serialize(answer_json);
            Send.SendData(resp, data);
        }

        void IAnswerGET.GET_ALL_BOOKS(HttpListenerResponse resp, SqlConnection con)
        {
            Get_All_About_Books get_All_About_Books = new Get_All_About_Books();
            get_All_About_Books.JSON_All_Books(resp, con);
        }

        void IAnswerGET.GET_BOOK(HttpListenerResponse resp, SqlConnection con, string urlRequest, string param)
        {
            Get_All_About_Books get_All_About_Books = new Get_All_About_Books();
            get_All_About_Books.JSON_GET_BOOK(resp, con, urlRequest, param);
        }

        void IAnswerGET.SEARCH_BY_INFO(HttpListenerResponse resp, SqlConnection con, string urlRequest)
        {
            Get_All_About_Books get_All_About_Books = new Get_All_About_Books();
            get_All_About_Books.JSON_SEARCH_BOOK(resp, con, urlRequest);
        }

        void IAnswerGET.GET_CLIENTS_LIST(HttpListenerResponse resp, SqlConnection con)
        {
            Get_About_Info_Client get_About_Info_Client = new Get_About_Info_Client();
            get_About_Info_Client.SEND_AND_SERIALIZE_JSON(resp, con, "");
        }

        void IAnswerGET.SEARCH_CLIENT(HttpListenerResponse resp, SqlConnection con, string urlRequest)
        {
            Get_About_Info_Client get_About_Info_Client = new Get_About_Info_Client();
            get_About_Info_Client.SQL_SEARCH_CLIENT(resp, con, urlRequest);
        }
    }
}
