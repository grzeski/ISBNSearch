using System;
using isbn.Models;

namespace isbn.Clients
{
    public interface IClient
    {
        Product FindProduct(string isbn);

    }
}
