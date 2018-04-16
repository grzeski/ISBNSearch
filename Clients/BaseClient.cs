using System;
using HtmlAgilityPack;
using isbn.Converters.Models;
using isbn.Models;

namespace isbn.Clients
{
    public abstract class BaseClient : IClient
    {
        protected const string HOST = "http://example.com";
        protected HtmlWeb web;
        protected Uri productPage;

        protected BaseClient()
        {
            web = new HtmlWeb();
        }

        public abstract Product FindProduct(string isbn);
        protected abstract Tuple<decimal, string> GetPrice();

        protected Product CreateEmptyProduct()
        {
            var url = new Uri(HOST);
            var price = new Tuple<decimal, string>(0, ((char)CurrencySymbols.EUR).ToString());

            return new Product
            {
                Price = price,
                ProductPage = url
            };

        }
    }
}
