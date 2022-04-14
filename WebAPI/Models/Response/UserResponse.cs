using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class UserResponse
{
  [JsonPropertyName("user_id")]
  public string Id { get; set; } = null!;

  [JsonPropertyName("name")]
  public string Name { get; set; } = null!;

  [JsonPropertyName("role")]
  public string Role { get; set; } = null!;

  [JsonPropertyName("profile_image")]
  public string? ProfileImage { get; set; }

  public UserResponse(User user)
  {
    this.Id = user.Id;
    this.Name = user.Name;
    this.Role = user.Role;
    this.ProfileImage = user.ProfileImage;
  }
}