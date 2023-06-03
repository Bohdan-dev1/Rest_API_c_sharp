using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Text.Json;
using System.Net;

using Beck_end_lib.Get_Sort_Requests.SEND;


namespace Beck_end_lib.Get_Sort_Requests.GET
{
    public class About_Order_Serialize
    {
        public int? ID_Order { get; set; }
        public string? Name_book { get; set; }
        public string? Date_Get_order { get; set; }
        public string? Date_Return_Book { get; set; }
        public string? Date_Need_Return { get; set; }

    }

    public class About_Client_Serialize
    {
        public int? ID_client { get; set; }
        public string? Full_name_client { get; set; }
        public string? Number_Phone_client { get; set; }
        public string? Email_Client { get; set; }
        public string? Address_client { get; set; }
        public string? NIE_code_client { get; set; }
        public List<About_Order_Serialize>? List_books { get; set; }

    }
    internal class Get_About_Info_Client
    {
        public string sql_script = @"SELECT 
dbo.Reader_Person.Person_ID, dbo.Issued_Books.ID_Order, dbo.Reader_Person.Full_Name, dbo.Reader_Person.Numer_Phone, dbo.Reader_Person.Email, 
dbo.Reader_Person.Address, dbo.Reader_Person.NIE_Code, dbo.Reader_Person.Add_Details_Person, dbo.Book.Name_Book, dbo.Issued_Books.Date_Get, dbo.Issued_Books.Date_Return,
dbo.Issued_Books.Date_Need_Return
FROM dbo.Reader_Person
JOIN dbo.Issued_Books 
ON dbo.Issued_Books.ID_Reader = dbo.Reader_Person.Person_ID
JOIN dbo.Book
ON dbo.Book.ID_Book = dbo.Issued_Books.ID_book;";

        public string sql_select_info_client = @"SELECT 
dbo.Issued_Books.ID_Order, dbo.Book.Name_Book, dbo.Issued_Books.Date_Get, dbo.Issued_Books.Date_Return,  dbo.Issued_Books.Date_Need_Return
FROM dbo.Reader_Person
JOIN dbo.Issued_Books 
ON dbo.Issued_Books.ID_Reader = dbo.Reader_Person.Person_ID
JOIN dbo.Book
ON dbo.Book.ID_Book = dbo.Issued_Books.ID_book
WHERE dbo.Reader_Person.Person_ID = ";

        string convet_string_for_like(string s)
        {
            string[] temp = s.Split("%22");
            temp = temp[1].Split("%20");
            string res2 = "%";
            if (temp.Length != 1)
            {
                foreach (string e in temp)
                {
                    res2 += e + "%";
                }
                return res2;
            }
            else return "%" + temp[0] + "%";
            
        }

            List<About_Order_Serialize> get_list_orders_clien(int client_ID, HttpListenerResponse resp)
        {
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBconfingLink"].ConnectionString);
            sqlConnection.Open();
            List<About_Order_Serialize> list_order_person = new List<About_Order_Serialize>();
            string sql_select = sql_select_info_client + client_ID.ToString() + ";";

            try
            {
                SqlCommand sqlCommandRun = new SqlCommand(sql_select, sqlConnection);
                SqlDataReader reader = sqlCommandRun.ExecuteReader();
                while (reader.Read())
                {
                    list_order_person.Add(new About_Order_Serialize
                    {
                        ID_Order = (int)reader["ID_Order"],
                        Name_book = (string)reader["Name_Book"],
                        Date_Get_order = reader["Date_Get"].ToString(),
                        Date_Return_Book = reader["Date_Return"].ToString(),
                        Date_Need_Return = reader["Date_Need_Return"].ToString(),

                    });
                }
                sqlConnection.Close();

            }
            catch (Exception)
            {
                sqlConnection.Close();
                return list_order_person;
            }


            return list_order_person;
        }
        public void SEND_AND_SERIALIZE_JSON(HttpListenerResponse resp, SqlConnection con, string customSQL)
        {
            SqlCommand sqlCommandRun = new SqlCommand(customSQL == "" ? sql_script: customSQL, con);
            List<About_Client_Serialize> list_cliens_data = new List<About_Client_Serialize>();

            try
            {
                SqlDataReader reader = sqlCommandRun.ExecuteReader();
                int coutCreating = 1, lastID = 1;
                while (reader.Read())
                {
                    if (coutCreating == 1)
                    {
                        list_cliens_data.Add(new About_Client_Serialize
                        {
                            ID_client = (int)reader["Person_ID"],
                            Full_name_client = (string)reader["Full_Name"],
                            Number_Phone_client = (string)reader["Numer_Phone"],
                            Email_Client = (string)reader["Email"],
                            Address_client = (string)reader["Address"],
                            NIE_code_client = (string)reader["NIE_Code"],
                            List_books = get_list_orders_clien((int)reader["Person_ID"], resp)
                        });
                        lastID = (int)reader["Person_ID"];
                        coutCreating++;
                    }
                    if (lastID != (int)reader["Person_ID"])
                    {
                        coutCreating = 1;
                        list_cliens_data.Add(new About_Client_Serialize
                        {
                            ID_client = (int)reader["Person_ID"],
                            Full_name_client = (string)reader["Full_Name"],
                            Number_Phone_client = (string)reader["Numer_Phone"],
                            Email_Client = (string)reader["Email"],
                            Address_client = (string)reader["Address"],
                            NIE_code_client = (string)reader["NIE_Code"],
                            List_books = get_list_orders_clien((int)reader["Person_ID"], resp)
                        });
                    }
                }


                string data = JsonSerializer.Serialize(list_cliens_data);
                Send.SendData(resp, data);
            }
            catch (Exception)
            {
                Send.SendData(resp, "{\"Error\" : \"Incorrect URL\"}");
                return;
            }
        }



        public void SQL_SEARCH_CLIENT(HttpListenerResponse resp, SqlConnection con, string url)
        {
            string[] request = url.Split('?');
            string[] stringReq = request[1].Split("&");
            string resAddRequest = sql_script[..^1];


            string[] tempR = stringReq[0].Split("=");
            switch (tempR[0])
            {
                case "Name_client":
                    resAddRequest += " WHERE dbo.Reader_Person.Full_Name LIKE '" + convet_string_for_like(tempR[1]) + "';";
                    break;
                case "Email_client":
                    resAddRequest += " WHERE dbo.Reader_Person.Email LIKE '" + convet_string_for_like(tempR[1]) + "';";
                    break;
                case "NIE_client":
                    resAddRequest += " WHERE dbo.Reader_Person.NIE_Code LIKE '" + convet_string_for_like(tempR[1]) + "';";
                    break;
                case "Phone_client":
                    resAddRequest += " WHERE dbo.Reader_Person.Numer_Phone LIKE '" + convet_string_for_like(tempR[1]) + "';";
                    break;
                case "Client_Address":
                    resAddRequest += " WHERE dbo.Reader_Person.Address LIKE '" + convet_string_for_like(tempR[1]) + "';";
                    break;
                case "By_Client_ID":
                    resAddRequest += " WHERE dbo.Reader_Person.Person_ID = '" + tempR[1] + "';";
                    break;
            }

            SEND_AND_SERIALIZE_JSON(resp, con, resAddRequest);
        }
    }
}
