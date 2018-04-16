namespace isbn.Providers
{
    public interface IProvider
    {
        string GetIsbnByTitle(string title);
    }
}
