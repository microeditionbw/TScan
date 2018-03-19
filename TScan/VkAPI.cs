using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Forms;
using xNet;

namespace TScan
{
    class VkAPI
    {

        private const string __APPID = "6383029";  //ID приложения
        private const string __VKAPIURL = "https://api.vk.com/method/";  //Ссылка для запросов
        private string _Token;  //Токен, использующийся при запросах

        public VkAPI(string AccessToken)
        {
            _Token = AccessToken;
        }
        HttpRequest Request = new HttpRequest();
        public Dictionary<string, string> GetInformation(string UserId, string[] Fields)  //Получение заданной информации о пользователе с заданным ID 
        {
           
            Request.AddUrlParam("&v=5.5&user_ids", UserId);
            Request.AddUrlParam("access_token", _Token);
            string Params = "";
            foreach (string i in Fields)
            {
                Params += i + ",";
            }
            Params = Params.Remove(Params.Length - 1);
            Request.AddUrlParam("fields", Params);
            string Result = Request.Get(__VKAPIURL + "users.get").ToString();
            Result = Result.Substring(13, Result.Length - 15);
            Dictionary<string, string> Dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(Result);
            return Dict;
        }

        public string GetCityById(string CityId)  //Перевод ID города в название
        {
     
            Request.AddUrlParam("&v=5.5&city_ids", CityId);
            Request.AddUrlParam("access_token", _Token);
            string Result = Request.Get(__VKAPIURL + "database.getCitiesById").ToString();
            Result = Result.Substring(13, Result.Length - 15);
            Dictionary<string, string> Dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(Result);
            return Dict["name"];
        }

        public string GetCountryById(string CountryId)  
        {
           
            Request.AddUrlParam("&v=5.5&country_ids", CountryId);
            Request.AddUrlParam("access_token", _Token);
            string Result = Request.Get(__VKAPIURL + "database.getCountriesById").ToString();
            Result = Result.Substring(13, Result.Length - 15);
            Dictionary<string, string> Dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(Result);
            return Dict["name"];
        }
   
        public dynamic GetCountries()
        {
            string Result = Request.Get(__VKAPIURL+"database.getCountries?&need_all=1&count=1000&v=5.73").ToString();
            dynamic results = JsonConvert.DeserializeObject<dynamic>(Result);

            return results;
        }
        public dynamic GetCountryIdByName(dynamic dynamic, string name)
        {
            dynamic results = dynamic;
            string getName = "";
            foreach (var i in results.response.items)
            {
                if (i.title.ToString() == name)
                {
                    getName = i.id.ToString();
                }
            }
            return getName;
        }
        public dynamic GetCitiesById(string id, string query)
        {
            string Result = Request.Get(__VKAPIURL + "database.getCities?&v=5.5&country_id=" + id+"&q="+query+"&need_all=0&count=100").ToString();
            dynamic results = JsonConvert.DeserializeObject<dynamic>(Result);
            return results;
        }
        public dynamic GetPeopleFromSearch(string country, string city, string count, string offset)
        {
            string Result = Request.Get(__VKAPIURL + "users.search?&v=5.73&offset=" + offset + "&country_id=" + country + "&city=" + city + "&count=" + count + "&access_token=" + _Token).ToString();
            dynamic results = JsonConvert.DeserializeObject<dynamic>(Result);
            return results;
        }

        public dynamic GetPeopleWall(string id, string count)
        {
            string Result = Request.Get(__VKAPIURL + "wall.get?&v=5.5&domain=" + id+"&count="+count+ "&filter=owner&extended=1&access_token=" + _Token).ToString();
            dynamic results = JsonConvert.DeserializeObject<dynamic>(Result);
            return results;
        }

        public dynamic GetPeopleWallSearch(string id, string q,string count,int offset)
        {
            string Result = Request.Get(__VKAPIURL + "wall.search?owner_id=" + id.Substring(2) + "&query=" + q+ "&owners_only=1&count=" + count + "&offset=" + offset.ToString() + "&extended=0&v=5.73&access_token=" + _Token).ToString();
            dynamic results = JsonConvert.DeserializeObject<dynamic>(Result);
            return results;
        }
    }
}
