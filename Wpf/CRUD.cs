using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Wpf
{
    static class CRUD
    {
        private const string url = "https://localhost:44335/";

        public static async Task Create(string uri, string data)
        {
            WebRequest request = WebRequest.Create(url + uri);
            request.ContentType = "application/json";
            request.Method = "POST";

            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = data;
                streamWriter.Write(json);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
            }
        }

        public static async Task<string> Read(string uri)
        {
            WebClient client = new WebClient();
            string result = "";

            using (Stream stream = client.OpenRead(url + uri))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        result = line;
                    }
                }
            }
            return result;
        }

        public static async Task Delete(string uri, string value)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                var response = client.DeleteAsync(uri + "/" + value).Result;
            }
        }
    }
}
