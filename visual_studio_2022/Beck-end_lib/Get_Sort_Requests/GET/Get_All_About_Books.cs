using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Net;

using Beck_end_lib.Get_Sort_Requests.SEND;
using System.Text;

namespace Beck_end_lib.Get_Sort_Requests.GET
{
    public class About_Books_Serialize
    {
        public int? ID_Book { get; set; }
        public string? Name_Book { get; set; }
        public int? How_many_books { get; set; }
        public string? Full_Name_Author { get; set; }
        public string? Yeer_Public { get; set; }
        public List<string>? Ahter_books { get; set; }

    }

    public class Ather_Book_List
    {

        public List<string> json_to_list(string jsonString)
        {
            JObject jObject = JObject.Parse(jsonString);
            List<string> listName = new List<string>();

            foreach (JValue array in jObject["List_books"]) listName.Add((string)array);

            return listName;
        }
    }




    internal class Get_All_About_Books
    {
        public string sql_script = @"SELECT 
dbo.Book.ID_Book, dbo.Book.Name_Book, dbo.Book.Year_Public , dbo.About_Author.Full_name, dbo.About_Author.List_Books, dbo.Number_Of_Books.How_many
FROM dbo.Book
JOIN dbo.About_Author 
ON dbo.About_Author.ID_Author = dbo.Book.ID_Author
JOIN dbo.Number_Of_Books
ON dbo.Number_Of_Books.ID_Book = dbo.Book.ID_Book;";
        public string sql_script_answer = @"SELECT 
dbo.Book.ID_Book, dbo.Book.Name_Book, dbo.Book.Year_Public , dbo.About_Author.Full_name, dbo.About_Author.List_Books, dbo.Number_Of_Books.How_many
FROM dbo.Book
JOIN dbo.About_Author 
ON dbo.About_Author.ID_Author = dbo.Book.ID_Author
JOIN dbo.Number_Of_Books
ON dbo.Number_Of_Books.ID_Book = dbo.Book.ID_Book
WHERE";
        public bool answer = false;

        public void JSON_All_Books(HttpListenerResponse resp, SqlConnection con)
        {

            SqlCommand sqlCommand = new SqlCommand(
                !this.answer ?
                this.sql_script :
                this.sql_script_answer,
                con);


            try
            {
                SqlDataReader sqlReader = sqlCommand.ExecuteReader();
                Ather_Book_List ather_Book_List = new Ather_Book_List();
                List<About_Books_Serialize> all_books = new List<About_Books_Serialize>();

                while (sqlReader.Read())
                {

                    all_books.Add(new About_Books_Serialize
                    {
                        ID_Book = (int)sqlReader["ID_Book"],
                        Name_Book = (string)sqlReader["Name_Book"],
                        How_many_books = (int)sqlReader["How_many"],
                        Full_Name_Author = (string)sqlReader["Full_name"],
                        Yeer_Public = sqlReader["Year_Public"].ToString(),
                        Ahter_books = ather_Book_List.json_to_list((string)sqlReader["List_Books"])

                    });
                }

                string data = JsonSerializer.Serialize(all_books);
                Send.SendData(resp, data);
            }
            catch (Exception e)
            {
                Send.SendData(resp, "{\"Error\" : \"Incorrect URL\"}");
                return;
            }


        }


        public void JSON_GET_BOOK(HttpListenerResponse resp, SqlConnection con, string urlRequest, string param)
        {
            string[] request = urlRequest.Split('?');
            string[] stringReq = request[1].Split("&");
            string resAddRequest = "";
            int iterator = 0;

            foreach (string elem in stringReq)
            {
                string[] tempR = elem.Split("=");
                switch (tempR[0])
                {
                    case "name_athor":
                        // symbol (")
                        string[] temp = tempR[1].Split("%22");
                        temp = temp[1].Split("%20");
                        string res = "";
                        if (temp.Length != 1)
                        {
                            foreach (string e in temp)
                            {
                                res += e + " ";
                            }
                            res = res[..^1];
                        }
                        else res = temp[0];


                        resAddRequest += " CONVERT(VARCHAR, dbo.About_Author.Full_name) = '" + res + "'";
                        break;

                    case "name_book":
                        // symbol (")
                        string[] temp2 = tempR[1].Split("%22");
                        temp2 = temp2[1].Split("%20");
                        string res2 = "";
                        if (temp2.Length != 1)
                        {
                            foreach (string e in temp2)
                            {
                                res2 += e + " ";
                            }
                            res2 = res2[..^1];
                        }
                        else res2 = temp2[0];

                        resAddRequest += " CONVERT(VARCHAR, dbo.Book.Name_Book) = '" + res2 + "'";
                        break;

                    case "year":
                        resAddRequest += " dbo.Book.Year_Public = " + tempR[1].ToString();
                        break;

                    case "id_book":
                        resAddRequest += " dbo.Book.ID_Book = " + tempR[1].ToString();
                        break;


                }
                iterator++;
                if (iterator < stringReq.Length) resAddRequest += (" " + param + " ");
            }
            resAddRequest += ";";

            this.answer = true;
            this.sql_script_answer += resAddRequest;
            JSON_All_Books(resp, con);
        }

        public void JSON_SEARCH_BOOK(HttpListenerResponse resp, SqlConnection con, string urlRequest)
        {
            string[] request = urlRequest.Split('?');
            string[] stringReq = request[1].Split("&");
            string resAddRequest = "";


            string[] tempR = stringReq[0].Split("=");
            switch (tempR[0])
            {
                case "name_athor":
                    // symbol (")
                    string[] temp = tempR[1].Split("%22");
                    temp = temp[1].Split("%20");
                    string res = "%";
                    if (temp.Length != 1)
                    {
                        foreach (string e in temp)
                        {
                            res += e + "%";
                        }
                        res += "%";
                    }
                    else res = temp[0] + "%";


                    resAddRequest += " CONVERT(VARCHAR, dbo.About_Author.Full_name) LIKE '" + res + "'";
                    break;

                case "name_book":
                    // symbol (")
                    string[] temp2 = tempR[1].Split("%22");
                    temp2 = temp2[1].Split("%20");
                    string res2 = "%";
                    if (temp2.Length != 1)
                    {
                        foreach (string e in temp2)
                        {
                            res2 += e + "%";
                        }
                        res2 += "%";
                    }
                    else res2 = temp2[0] + "%";

                    byte[] bytes = Encoding.Default.GetBytes(res2);
                    res2 = Encoding.UTF8.GetString(bytes);

                    resAddRequest += " CONVERT(VARCHAR, dbo.Book.Name_Book) LIKE '" + res2 + "'";
                    break;

                case "year":
                    string[] temp3 = tempR[1].Split("_");
                    if (temp3.Length == 1)
                    {
                        resAddRequest += " dbo.Book.Year_Public = " + temp3[0].ToString();
                        break;
                    }
                    else
                    {

                        resAddRequest += " dbo.Book.Year_Public BETWEEN ";

                        try
                        {
                            int startValue = int.Parse(temp3[0]);
                            int endValue = int.Parse(temp3[1]);

                            if (startValue > endValue) resAddRequest += temp3[1] + (" AND ") + temp3[0];
                            else resAddRequest += temp3[0] + (" AND ") + temp3[1];
                        }
                        catch (Exception)
                        {
                            Send.SendData(resp, "{\"Error\" : \"Incorrect request \"}");
                            return;
                        }


                    }
                    break;

                case "id_book":
                    string[] tempB = tempR[1].Split("_");
                    if (tempB.Length == 1)
                    {
                        resAddRequest += " dbo.Book.ID_Book = " + tempR[1].ToString();
                        break;
                    }

                    try
                    {
                        int startValueB = int.Parse(tempB[0]);
                        int endValueB = int.Parse(tempB[1]);
                        resAddRequest += " dbo.Book.ID_Book BETWEEN ";

                        if (startValueB > endValueB) resAddRequest += tempB[1] + (" AND ") + tempB[0];
                        else resAddRequest += tempB[0] + (" AND ") + tempB[1];
                    }
                    catch (Exception)
                    {
                        Send.SendData(resp, "{\"Error\" : \"Incorrect request \"}");
                        return;
                    }


                    break;
            }
            resAddRequest += ";";

            if (resAddRequest == ";")
            {
                Send.SendData(resp, "{\"Error\" : \"Incorrect URL\"}");
                return;
            }

            this.answer = true;
            this.sql_script_answer += resAddRequest;
            JSON_All_Books(resp, con);

        }
    }
}

