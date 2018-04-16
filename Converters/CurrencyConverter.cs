using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using isbn.Converters.Models;

namespace isbn.Models
{
    public class CurrencyConverter
    {
        const string NBP_API = "http://api.nbp.pl/api/exchangerates/tables/A?format=json";

        public decimal ToPLN(Product product)
        {
            var currencySymbol = product.Price.Item2;
            var currencyCode = Enum.GetName(typeof(CurrencySymbols), currencySymbol.First());
            return Decimal.Round(GetCurrencyRate(currencyCode) * product.Price.Item1, 2);

        }

        JObject FetchExchangeRates()
        {
            var client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(NBP_API).Result;
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            content = content.Substring(1, content.Length - 2);
            return JObject.Parse(content);
        }

        decimal GetCurrencyRate(string currencyCode)
        {
            JObject ratesTable = FetchExchangeRates();
            var rates = ratesTable["rates"];
            var currencies = JsonConvert.DeserializeObject < List<Currency>>(rates.ToString());
            return currencies.Find(x => x.Code.Equals(currencyCode)).Mid;
        }

    }
}
