using software_studio_backend.Shared;
using software_studio_backend.Services;
using software_studio_backend.Models;
using software_studio_backend.Utils;
namespace software_studio_backend.Middlewares;

public class Middleware
{

  public static void RequireAuthHandler(IApplicationBuilder app)
  {
    app.Use(DeserializeUser);
    // app.Use(UserCheck);
    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
    });
  }

  async static Task DeserializeUser(HttpContext context, RequestDelegate next)
  {
    Console.WriteLine("\nThough DeserializeUser middleware\n");
    string? accessToken = context.Request.Cookies[Constant.Name.AccessToken];

    if (accessToken == null) { await next(context); return; }

    Token? accSessionToken = SessionCollection.GetSession(accessToken);

    if (accSessionToken == null) { await next(context); return; }

    if (accSessionToken.IsExpired)
    {

      // Try refresh the accessToken
      // Get refresh Token from request cookies
      string? refreshToken = context.Request.Cookies[Constant.Name.RefreshToken];

      // check refresh token is exist.
      if (refreshToken == null) { await next(context); return; }

      // 
      Token? refSessionToken = SessionCollection.GetSession(refreshToken);

      if (refSessionToken == null || refSessionToken.IsExpired) { await next(context); return; }

      string newAccessToken = TokenUtils.GenerateAccessToken(refSessionToken.User);

      Token newToken = new Token { Type = Constant.Name.AccessToken, User = refSessionToken.User, EncryptedString = newAccessToken, Expires = DateTime.Now.AddSeconds(Constant.Number.AccessTokenExpiresInSec) };

      //WARNING ------------_!!!!!!!!
      context.Response.Cookies.Append(Constant.Name.AccessToken, newAccessToken, new CookieOptions
      {
        HttpOnly = true,
        Expires = DateTime.Now.AddSeconds(Constant.Number.AccessTokenExpiresInSec)
      });
      // Not Sure --------------!!!!!

      SessionCollection.Invalidate(accSessionToken);
      SessionCollection.Create(newToken);
    }



    // verify Token 
    await next(context);

  }
}