using Newtonsoft.Json.Linq;

namespace Beck_end_lib.Get_Sort_Requests.Serializes.UserGet
{
    public class JSON_USER_SERIALIZE
    {
        public string? name { get; set; }
    }
    public class USER_SERIALIZE_CLASS
    {
        public string? Date_Add_DB { get; set; }
        public JSON_USER_SERIALIZE? about_User { get; set; }
    }

    public class USER_SERIALIZE
    {

        public JSON_USER_SERIALIZE GET_JUS(string userDataJSON)
        {
            JObject jObject = JObject.Parse(userDataJSON);
            JSON_USER_SERIALIZE userOBJ = new JSON_USER_SERIALIZE
            {
                name = jObject["name"].ToString(),
            };

            return userOBJ;
        }
    }
  
}
