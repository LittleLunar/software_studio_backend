using software_studio_backend.Utils;
using software_studio_backend.Shared;
using software_studio_backend.Models;
using software_studio_backend.Services;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using MongoDB.Driver;
using System.Net.Http.Headers;

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

    // Debugging if client have no accessToken
    if (accessToken.Length <= 10) accessToken = String.Empty;

    if (!String.IsNullOrEmpty(accessToken)) { await _next(context); return; }

    bool isValid = TokenUtils.ValidateToken(accessToken);

    if (isValid) { await _next(context); return; }

    string? refreshToken = context.Request.Cookies[Constant.Name.RefreshToken];

    Console.WriteLine("RefreshToken : {0}", refreshToken);

    if (String.IsNullOrEmpty(refreshToken)) { await _next(context); return; }

    bool isRefreshValid = TokenUtils.ValidateToken(refreshToken);

    if (!isRefreshValid) { await _next(context); return; }

    User user = await DeserializeUser(refreshToken);

    string newAccessToken = TokenUtils.GenerateAccessToken(user);

    context.Request.Headers.Authorization = "Bearer " + newAccessToken;

    context.Response.Cookies.Append(Constant.Name.AccessToken, newAccessToken, new CookieOptions
    {
      HttpOnly = false,
      Expires = DateTime.Now.AddSeconds(Constant.Number.AccessTokenExpiresInSec)
    });

    await _next(context);
  }

  private async Task<User> DeserializeUser(string token)
  {
    string data = new JwtSecurityTokenHandler().ReadJwtToken(token).Payload.SerializeToJson();

    JObject jsonData = JObject.Parse(data);

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == jsonData[ClaimTypes.Name]!.ToString()).FirstOrDefaultAsync();

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
