using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class UserResponse
{
  public string Id { get; set; } = null!;

  public string Name { get; set; } = null!;

  [JsonPropertyName("profile_image")]
  public string? ProfileImage { get; set; }

  public UserResponse(User user)
  {
    this.Id = user.Id;
    this.Name = user.Name;
    this.ProfileImage = user.ProfileImage;
  }
}