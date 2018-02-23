using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xNet;

namespace TScan
{
    class VkAPI
    {
        private const string __APPID = "ApplicationId";  //ID приложения
        private const string __VKAPIURL = "https://api.vk.com/method/";  //Ссылка для запросов
        private string _Token;  //Токен, использующийся при запросах

        public VkAPI(string AccessToken)
        {
            _Token = AccessToken;
        }

        public Dictionary<string, string> GetInformation(string UserId, string[] Fields)  //Получение заданной информации о пользователе с заданным ID 
        {
            HttpRequest GetInformation = new HttpRequest();
            GetInformation.AddUrlParam("user_ids", UserId);
            GetInformation.AddUrlParam("access_token", _Token);
            string Params = "";
            foreach (string i in Fields)
            {
                Params += i + ",";
            }
            Params = Params.Remove(Params.Length - 1);
            GetInformation.AddUrlParam("fields", Params);
            string Result = GetInformation.Get(__VKAPIURL + "users.get").ToString();
            Result = Result.Substring(13, Result.Length - 15);
            Dictionary<string, string> Dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(Result);
            return Dict;
        }

        public string GetCityById(string CityId)  //Перевод ID города в название
        {
            HttpRequest GetCityById = new HttpRequest();
            GetCityById.AddUrlParam("city_ids", CityId);
            GetCityById.AddUrlParam("access_token", _Token);
            string Result = GetCityById.Get(__VKAPIURL + "database.getCitiesById").ToString();
            Result = Result.Substring(13, Result.Length - 15);
            Dictionary<string, string> Dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(Result);
            return Dict["name"];
        }

        public string GetCountryById(string CountryId)  //Перевод ID страны в название
        {
            HttpRequest GetCountryById = new HttpRequest();
            GetCountryById.AddUrlParam("country_ids", CountryId);
            GetCountryById.AddUrlParam("access_token", _Token);
            string Result = GetCountryById.Get(__VKAPIURL + "database.getCountriesById").ToString();
            Result = Result.Substring(13, Result.Length - 15);
            Dictionary<string, string> Dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(Result);
            return Dict["name"];
        }

        public dynamic GetCountries()
        {
            HttpRequest GetCountries = new HttpRequest();
            string Result = GetCountries.Get("https://api.vk.com/api.php?oauth=1&method=database.getCountries&v=5.5&need_all=1&count=1000").ToString();
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
            HttpRequest GetCitiesById = new HttpRequest();
            string Result = GetCitiesById.Get("https://api.vk.com/api.php?oauth=1&method=database.getCities&v=5.5&country_id="+id+"&q="+query+"&need_all=0&count=100").ToString();
            dynamic results = JsonConvert.DeserializeObject<dynamic>(Result);
            return results;
        }
        public dynamic GetPeopleFromSearch(string country, string city, string count, string offset)
        {
            HttpRequest GetCitiesById = new HttpRequest();
            string Result = GetCitiesById.Get("https://api.vk.com/api.php?oauth=1&method=users.search&v=5.5&offset="+offset+"&country_id=" + country + "&city=" + city + "&count="+count+"&access_token="+ _Token).ToString();
            dynamic results = JsonConvert.DeserializeObject<dynamic>(Result);
            return results;
        }

        public dynamic GetPeopleWall(string id, string count)
        {
            HttpRequest GetPeopleWall = new HttpRequest();
            string Result = GetPeopleWall.Get("https://api.vk.com/api.php?oauth=1&method=wall.get&v=5.5&domain="+id+"&count="+count+ "&filter=owner&extended=1&access_token=" + _Token).ToString();
            dynamic results = JsonConvert.DeserializeObject<dynamic>(Result);
            return results;
        }
    }
}
