using System.Text;
using software_studio_backend.Shared;
using software_studio_backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace software_studio_backend.Utils;

public class TokenUtils
{
  public static TokenValidationParameters tokensValidatorParam { get; } = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = Configuration.staticConfig["Jwt:Issuer"],
    ValidAudience = Configuration.staticConfig["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.staticConfig["Jwt:SecretKey"])),
    ClockSkew = TimeSpan.Zero
  };
  public static JwtSecurityTokenHandler tokenHandler { get; } = new JwtSecurityTokenHandler();
  public static string GenerateAccessToken(User user)
  {
    Claim[] claims = new Claim[] {
      new Claim("username", user.Username),
      new Claim("role", user.Role),
      new Claim("display_name", user.Name),
    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
      new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddSeconds(Constant.Number.AccessTokenExpiresInSec).ToString())
    };

    return GenerateToken(DateTime.Now.AddSeconds(Constant.Number.AccessTokenExpiresInSec), claims);

  }

  public static string GenerateRefreshToken(User user)
  {
    Claim[] claims = new Claim[] {
      new Claim("username", user.Username)
    };
    return GenerateToken(DateTime.Now.AddMonths(Constant.Number.RefreshTokenExpiresInMonths), claims);
  }

  private static string GenerateToken(DateTime expires, Claim[]? claims = null)
  {
    SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.staticConfig["Jwt:SecretKey"]));
    SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    JwtSecurityToken token = new JwtSecurityToken(
      Configuration.staticConfig["Jwt:Issuer"],
      Configuration.staticConfig["Jwt:Audience"],
      claims,
      expires: expires,
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public static bool ValidateToken(string token)
  {
    try
    {
      tokenHandler.ValidateToken(token, tokensValidatorParam, out SecurityToken validatedToken);
    }
    catch (Exception)
    {
      return false;
    }
    return true;
  }
}