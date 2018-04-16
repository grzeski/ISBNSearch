using System;

namespace isbn.Models
{
    public class Product
    {
        Tuple<decimal, string> price;
        Uri productPage;
        decimal convertedPrice;

        public Tuple<decimal, string> Price { get => price; set => price = value; }
        public Uri ProductPage { get => productPage; set => productPage = value; }
        public decimal ConvertedPrice { get => convertedPrice; set => convertedPrice = value; }
    }
}
