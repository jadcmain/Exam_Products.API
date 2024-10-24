using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Products.API.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Products.API.Endpoints;

public static class AuthenticationEndpoints
{
    public static RouteGroupBuilder MapAuthentication(this RouteGroupBuilder group)
    {
        group.MapPost("/", GenerateToken);
        return group;
    }

    static async Task<Results<UnauthorizedHttpResult, Ok<TokenResponseModel>>> GenerateToken(TokenAuthModel data, IOptions<AppSettings> appSettings)
    {
        if (data != null)
        {
            if (data.TokenAuthUser != null & data.TokenAuthPass != null)
            {
                if (data.TokenAuthUser == appSettings.Value.TokenUsername &&
                    data.TokenAuthPass == appSettings.Value.TokenPassword)
                {
                    // start creation of token
                    var claims = new[]
                            {
                                new Claim(ClaimTypes.Name, data.TokenAuthUser ?? ""),
                                new Claim(ClaimTypes.Role, data.Role!)
                            };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.Key ?? ""));
                    var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                    var token = new JwtSecurityToken(
                                        claims: claims,
                                        expires: DateTime.Now.AddDays(appSettings.Value.TokenExpiryByDay),
                                        signingCredentials: cred
                                    );

                    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                    var tokenRes = new TokenResponseModel() { AuthToken = jwt, TokenExpiry = token.ValidTo };
                    return TypedResults.Ok(tokenRes);
                }
            }
        }

        return TypedResults.Unauthorized();
    }
}
