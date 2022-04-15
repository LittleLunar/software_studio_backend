using System.Security.Cryptography;
using System.Text;
using software_studio_backend.Shared;
namespace software_studio_backend.Utils;

public class PasswordEncryption
{
  public static string Encrypt(string password)
  {
    Byte[] textBytes = Encoding.UTF8.GetBytes(password);
    Byte[] keyBytes = Encoding.UTF8.GetBytes(Configuration.staticConfig["HmacSHA256:Key"]);

    Byte[] hashBytes;
    using (HMACSHA256 hash = new HMACSHA256(keyBytes))
    {
      hashBytes = hash.ComputeHash(textBytes);
    }
    string encrypted = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    return encrypted;
  }

  public static bool ValidatePassword(string pass1, string encryptedPass)
  {
    string encrypted1 = Encrypt(pass1);

    if (encrypted1 != encryptedPass)
      return false;

    return true;
  }
}