using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using isbn.Clients;
using isbn.Models;
using isbn.Providers;
using Microsoft.AspNetCore.Mvc;


namespace isbn.Controllers
{
    public class HomeController : Controller
    {
        readonly IProvider isbnProvider;

        public HomeController(IProvider provider)
        {
            isbnProvider = provider;
        }

        public IActionResult Index()
        {
            if (Request.Method.Equals(HttpMethod.Post.ToString()))
            {
                var title = Request.Form["title"];
                var isbn = isbnProvider.GetIsbnByTitle(title);

                var clients = new List<IClient> { new ApressClient(), new AmazonClient() };
                var books = new List<Product>();
                foreach (IClient client in clients)
                {
                    books.Add(client.FindProduct(isbn));
                }

                var currencyConverter = new CurrencyConverter();
                foreach (Product book in books)
                {
                    book.ConvertedPrice = currencyConverter.ToPLN(book);
                }

                var foundBooks = books.FindAll(p => p.ConvertedPrice > 0).OrderBy(x => x.ConvertedPrice);
                if (foundBooks.Any())
                {
                    ViewData["product"] = foundBooks.First();
                    ViewData["result"] = true;
                }
                else
                {
                    ViewData["result"] = false;
                    ViewData["message"] = "No books found";
                }

                return View();
            }

            ViewData["result"] = false;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
