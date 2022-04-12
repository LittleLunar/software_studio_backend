using System.Text.Json.Serialization;
namespace software_studio_backend.Models;

public class LogoutRequest
{
  [JsonPropertyName("access_token")]
  public string EncryptedString { get; set; } = null!;
}