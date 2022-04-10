using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using software_studio_backend.Shared;
using software_studio_backend.Models;
using software_studio_backend.Services;
using System.Security.Cryptography;

namespace software_studio_backend.Utils;

public static class TokenUtils
{
  public static string GenerateAccessToken(User user)
  {

    string accessToken = EncryptHMAC256(payload: user.Username);

    Token token = new Token { Type = Constant.Name.AccessToken, User = user, EncryptedString = accessToken, Expires = DateTime.Now.AddSeconds(Constant.Number.AccessTokenExpiresInSec) };

    SessionCollection.Create(token);

    return accessToken;
  }

  public static string GenerateRefreshToken(User user)
  {

    string refreshToken = EncryptHMAC256(payload: user.Id);

    Token token = new Token { Type = Constant.Name.RefreshToken, User = user, EncryptedString = refreshToken, Expires = DateTime.Now.AddMonths(Constant.Number.RefreshTokenExpiresInMonths) };

    SessionCollection.Create(token);

    return refreshToken;
  }

  private static string EncryptHMAC256(string payload)
  {
    if (Configuration.staticConfig == null)
      throw new NullReferenceException("No Secret Key Provided");

    string encrypted;
    
    using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(Configuration.staticConfig["Secret:SecretKey"])))
    {
      var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(payload + DateTime.Now.ToString()));
      encrypted = Convert.ToBase64String(hash);
    }

    return encrypted;
  }

}