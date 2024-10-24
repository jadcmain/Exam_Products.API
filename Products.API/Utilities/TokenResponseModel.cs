namespace Products.API.Utilities;

public class TokenResponseModel
{
    public string AuthToken { get; set; } = string.Empty;
    public DateTime TokenExpiry { get; set; }
}
