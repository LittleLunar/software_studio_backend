namespace software_studio_backend.Shared;

public static class Constant
{
  public static class Name
  {
    public const string AccessToken = "accessToken";
    public const string RefreshToken = "refreshToken";
  }

  public static class Number
  {
    public const double AccessTokenExpiresInSec = 15; // InSec type => double
    public const int RefreshTokenExpiresInMonths = 1; // InMonth type => int
  }

}