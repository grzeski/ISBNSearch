using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json.Linq;

namespace isbn.Providers
{
    public class GoogleIsbnProvider : IProvider
    {
        const string ISBN_API = "https://www.googleapis.com/books/v1/volumes?q=title:{0}&langRestrict=en";
        const string ITEMS = "items";
        const string VOLUME = "volumeInfo";
        const string INDUSTRY_IDS = "industryIdentifiers";
        const string IDENTIFIER = "identifier";

        Uri TargetApi;

        public string GetIsbnByTitle(string title)
        {
            SetTargetApi(title);
            return GetIsbn();
        }

        JObject FetchBookData()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(TargetApi).Result;
            response.EnsureSuccessStatusCode();
            return JObject.Parse(response.Content.ReadAsStringAsync().Result);
        }

        void SetTargetApi(string title)
        {
            Uri.TryCreate(String.Format(ISBN_API, HttpUtility.UrlEncode(title)), UriKind.Absolute, out TargetApi);
        }

        string ExtractIsbn(JObject data)
        {
            var book = data[ITEMS].First();
            var identifiers = book[VOLUME][INDUSTRY_IDS];

            return identifiers.First()[IDENTIFIER].ToString();
        }

        string GetIsbn()
        {
            var data = FetchBookData();
            return ExtractIsbn(data);
        }

    }
}
