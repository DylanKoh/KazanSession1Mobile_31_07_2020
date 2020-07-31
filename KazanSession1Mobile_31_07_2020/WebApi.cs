using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KazanSession1Mobile_31_07_2020
{
    public class WebApi
    {
        private HttpClient client = new HttpClient();
        string baseAddress = "http://10.0.2.2:49321/";
        public async Task<string> PostAsync(string urlName, string Json)
        {
            var newUrl = baseAddress + urlName;
            if (Json == null)
            {
                var stringContent = new StringContent("", Encoding.UTF8, "application/json");
                var responseString = await client.PostAsync(newUrl, stringContent).Result.Content.ReadAsStringAsync();
                return responseString;
            }
            else
            {
                var stringContent = new StringContent(Json, Encoding.UTF8, "application/json");
                var responseString = await client.PostAsync(newUrl, stringContent).Result.Content.ReadAsStringAsync();
                return responseString;
            }
        }
    }
}
