using software_studio_backend.Utils;
using software_studio_backend.Shared;
using software_studio_backend.Models;
using software_studio_backend.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using MongoDB.Driver;

namespace software_studio_backend.Middleware;

public class RevokeToken
{
  private readonly RequestDelegate _next;
  private readonly MongoDBService _mongoDB;
  public RevokeToken(RequestDelegate next, MongoDBService mongoDB)
  {
    _next = next;
    _mongoDB = mongoDB;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    // Check Token in request header Authorization section.
    string accessToken = context.Request.Headers.Authorization.ToString().Split(' ').Last();

    bool isValid = TokenUtils.ValidateToken(accessToken);

    if (isValid) { await _next(context); return; }

    string? refreshToken = context.Request.Cookies[Constant.Name.RefreshToken];

    if (String.IsNullOrEmpty(refreshToken)) { await _next(context); return; }

    bool isRefreshValid = TokenUtils.ValidateToken(refreshToken);

    if (!isRefreshValid) { await _next(context); return; }

    User? user = await DeserializeUser(refreshToken);

    if (user == null) { await _next(context); return; }

    string newAccessToken = TokenUtils.GenerateAccessToken(user);

    context.Request.Headers.Authorization = "Bearer " + newAccessToken;

    context.Response.Cookies.Append(Constant.Name.AccessToken, newAccessToken, new CookieOptions
    {
      HttpOnly = false,
      Expires = DateTime.UtcNow.AddDays(Constant.Number.AccessTokenExpiresInDay)
    });

    await _next(context);
  }

  private async Task<User> DeserializeUser(string token)
  {
    string data = new JwtSecurityTokenHandler().ReadJwtToken(token).Payload.SerializeToJson();
    // Console.WriteLine(data);

    JObject jsonData = JObject.Parse(data);

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == jsonData["username"]!.ToString() && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    return user;
  }
}

public static class RevokeTokenExtensions
{
  public static IApplicationBuilder UseRevokeToken(this IApplicationBuilder app)
  {
    return app.UseMiddleware<RevokeToken>();
  }

}
