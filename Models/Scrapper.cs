using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace isbn.Models
{
    public class Scrapper
    {
        private string g_api = "https://www.googleapis.com/books/v1/volumes?q=title:Pro%20JavaFX%209";
        private string amazon_url = "https://www.amazon.com/s/?field-keywords="; //9781484225141";
        private string apress_url = "https://www.apress.com/us/search?query="; //9781484225141";
        private string apress_host = "https://www.apress.com";
        private string amazon_whole_price = "//*[@class='sx-price-whole']";
        private string amazon_fractional_price = "//*[@class='sx-price-fractional']";
        private string apress_cover = "//*[@class='cover']";
        private string apress_price = ".//*[@class='price-box']/span[@class='price']";//"//*[@class='price']";
        public async void getIsbn()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(g_api);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject books = JObject.Parse(responseBody);
            var isbn = books["items"].First()["volumeInfo"]["industryIdentifiers"][0]["identifier"];
            //var numero_uno = books["items"][0]["industryIdentifiers"];
            //var vals = numero_uno["industryIdentifiers"].First();
            
            var web = new HtmlWeb();
            var doc = web.Load(apress_url + isbn);
            var node = doc.DocumentNode.SelectNodes(apress_cover).First();
            var next_page = node.Attributes["href"];
            var product_page = web.Load(apress_host + next_page.Value);
            var price = product_page.DocumentNode.SelectNodes(apress_price);
            string money = price.First().InnerHtml.Replace("\n", "").Trim();
        }

    }


}

