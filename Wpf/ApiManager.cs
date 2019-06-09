using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Wpf
{
    static class ApiManager
    {
        private const string url = "https://localhost:44335/";

        public static async Task Create(string uri, string value)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(value, Encoding.UTF8, "application/json");
                await client.PostAsync(url + uri, content);
            }
        }

        public static async Task<string> Read(string uri)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url + uri);
                return await response.Content.ReadAsStringAsync();
            }
        }

        public static async Task Change(string uri, string value)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(value, Encoding.UTF8, "application/json");
                await client.PutAsync(url + uri, content);
            }
        }

        public static async Task Delete(string uri, string value)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                await client.DeleteAsync(uri + "/" + value);
            }
        }
    }
}
