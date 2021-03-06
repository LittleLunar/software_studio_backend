
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class RegisterAdminRequest
{
  [Required]
  [JsonPropertyName("username")]
  public string Username { get; set; } = null!;

  [Required]
  [JsonPropertyName("password")]
  public string Password { get; set; } = null!;
}