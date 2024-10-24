namespace Products.API.Utilities;

public class AppSettings
{
    public string? Key { get; set; }
    public int TokenExpiryByDay { get; set; }
    public string? TokenUsername { get; set; }
    public string? TokenPassword { get; set; }
}
