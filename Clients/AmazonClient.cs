using System;
using System.Linq;
using isbn.Models;

namespace isbn.Clients
{
    public class AmazonClient : BaseClient
    {
        new const string HOST = "https://www.amazon.com";
        const string AMAZON_SEARCH = HOST + "/s/?field-keywords={0}";
        const string AMAZON_WHOLE_PRICE_XPATH = "//*[@id='result_0']/div/div/div/div[2]/div[2]/div[1]/div[2]/a/span[2]/span/span"; //"//*[@class='sx-price-whole']";
        const string AMAZON_FRACTIONAL_PRICE_XPATH = "//*[@id='result_0']/div/div/div/div[2]/div[2]/div[1]/div[2]/a/span[2]/span/sup[2]"; //"//*[@class='sx-price-fractional']";
        const string AMAZON_PRICE_CURRENCY_XPATH = "//*[@id='result_0']/div/div/div/div[2]/div[2]/div[1]/div[2]/a/span[2]/span/sup[1]";
        const string AMAZON_PRODUCT_PAGE_XPATH = ".//*[@class='a-link-normal s-access-detail-page  s-color-twister-title-link a-text-normal']";
        string ISBN = String.Empty;


        public override Product FindProduct(string isbn)
        {
            ISBN = isbn;
            var searchResults = web.Load(String.Format(AMAZON_SEARCH, ISBN));
            var productResult = searchResults.DocumentNode.SelectNodes(AMAZON_PRODUCT_PAGE_XPATH);
            if(productResult == null)
            {
                return CreateEmptyProduct();
            }    
                
            var productLink = productResult.First().GetAttributeValue("href", HOST);
            SetProductPage(productLink);

            var product = new Product
            {
                Price = GetPrice(),
                ProductPage = productPage
            };

            return product;
        }

        protected override Tuple<decimal, string> GetPrice()
        {
            var bookPage = web.Load(String.Format(AMAZON_SEARCH, ISBN));
            var currencySymbol = bookPage.DocumentNode.SelectNodes(AMAZON_PRICE_CURRENCY_XPATH).First().InnerHtml;
            var wholePrice = bookPage.DocumentNode.SelectNodes(AMAZON_WHOLE_PRICE_XPATH).First().InnerHtml;
            var fractionalPrice = bookPage.DocumentNode.SelectNodes(AMAZON_FRACTIONAL_PRICE_XPATH).First().InnerHtml;

            var price = String.Format("{0}.{1}", wholePrice, fractionalPrice);

            return new Tuple<decimal, string>(
                decimal.Parse(price),
                currencySymbol
            );
       
        }

        void SetProductPage(string productLink)
        {
            Uri.TryCreate(productLink, UriKind.Absolute, out productPage);
        }

    }
}
