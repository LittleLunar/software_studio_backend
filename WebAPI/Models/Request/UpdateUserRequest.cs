using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class UpdateUserRequest
{
  [JsonPropertyName("name")]
  public string? Name { get; set; }

  [JsonPropertyName("profile_image")]
  public string? ProfileImage { get; set; }
}