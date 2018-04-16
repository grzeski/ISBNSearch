using System;
using System.Linq;
using isbn.Models;

namespace isbn.Clients
{
    public class ApressClient : BaseClient
    {
        new const string HOST = "https://www.apress.com";
        const string APRESS_SEARCH = HOST + "/us/search?query={0}";
        string APRESS_COVER_XPATH = "//*[@class='cover']";
        string APRESS_PRICE_XPATH = ".//*[@class='price-box']/span[@class='price']";//"//*[@class='price']";

        public override Product FindProduct(string isbn)
        {
            var searchResults = web.Load(String.Format(APRESS_SEARCH, isbn));
            var productResult = searchResults.DocumentNode.SelectNodes(APRESS_COVER_XPATH);
            if (productResult == null)
            {
                return CreateEmptyProduct();
            }

            var productLink = productResult.First().Attributes["href"].Value;
            SetProductPage(productLink);

            var product = new Product
            {
                Price = GetPrice(),
                ProductPage = productPage
            };

            return product;

        }

        void SetProductPage(string productLink)
        {
            Uri.TryCreate(HOST + productLink, UriKind.Absolute, out productPage);
        }

        protected override Tuple<decimal, string> GetPrice()
        {
            var bookPage = web.Load(productPage);
            var price = bookPage.DocumentNode
                                    .SelectNodes(APRESS_PRICE_XPATH)
                                    .First()
                                    .InnerHtml;

            var result = CleanupPrice(price).Split(' ');

            return new Tuple<decimal, string>(
                decimal.Parse(result.First()),
                result.Last()
            );


        }

        string CleanupPrice(string price)
        {
            return price.Replace(Environment.NewLine, String.Empty)
                        .Trim()
                        .Replace(",", ".");
        }


    }
}
