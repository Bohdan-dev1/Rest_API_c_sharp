using System.Net;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

using Beck_end_lib.Get_Sort_Requests;
using Beck_end_lib.Get_Sort_Requests.Default;
using Beck_end_lib.Log_functions;



namespace Program
{
    public class HttpServer
    {
        private static SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBconfingLink"].ConnectionString);

        public static void Main(string[] args)
        {




            sqlConnection.Open();
            if (sqlConnection.State == ConnectionState.Open) { 
                Console.WriteLine("Connection DB");

                var listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:4444/");
                listener.Start();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Listening on port 4444.... ");
                sqlConnection.Close();

                LogClass logClass = new LogClass();
                logClass.log_function(("Log <" + DateTime.Now.ToString() + "> OK"), false, "none");

                while (true)
                {
                    sqlConnection.Open();
                    HttpListenerContext ctx = listener.GetContext();
                    
                    HttpListenerResponse resp = ctx.Response;
                    HttpListenerRequest request = ctx.Request;

                    string URL = request.RawUrl;
                    string rawURL = URL.Split('?')[0];
                    string requestMethod = request.HttpMethod.ToString();

                    resp.StatusCode = (int)HttpStatusCode.OK;
                    resp.StatusDescription = "Status, OK";
                    resp.Headers.Set("Content-Type", "text/plain");

                    Beck_end_lib.Get_Sort_Requests.IAnswerProcessor gsd = new GetSortDATA();

                    switch (requestMethod)
                    {
                        case "GET":
                            logClass.log_function(("Log <" + DateTime.Now.ToString() + "> (" + requestMethod + ") http://localhost:4444" + URL), false, requestMethod);
                            
                            gsd.IAprocessor(new GET_CLASS(), resp, sqlConnection, URL, rawURL);
                            break;



                        default:
                            Default_Request default_Request = new Default_Request();
                            default_Request.Undefined_URL(
                                resp, 
                                ("Not fount frunction processing for ( " + requestMethod + " )")
                            );

                            logClass.log_function(("Log <" + DateTime.Now.ToString() + "> (" + requestMethod + ") Not found function"), true, requestMethod);

                            break;
                    }
                        


                    sqlConnection.Close();
                }
            }

        }
    }
}

 