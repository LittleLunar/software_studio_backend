
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class AdminUserUpdateRequest
{
  [JsonPropertyName("username")]
  public string? Username { get; set; }
  [JsonPropertyName("password")]
  public string? Password { get; set; }

  [JsonPropertyName("display_name")]
  public string? Name { get; set; }

  [JsonPropertyName("role")]
  public string? Role { get; set; }

  [JsonPropertyName("banned")]
  public bool? Banned { get; set; }

  [JsonPropertyName("deleted")]
  public bool? Deleted { get; set; }

  [JsonPropertyName("profile_image")]
  public string? ProfileImage { get; set; }
}