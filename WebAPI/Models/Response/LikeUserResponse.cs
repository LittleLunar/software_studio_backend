using System.Text.Json.Serialization;
namespace software_studio_backend.Models;

public class LikeUserResponse
{
  [JsonPropertyName("username")]
  public string Username { get; set; } = null!;

  [JsonPropertyName("name")]
  public string Name { get; set; } = null!;
}