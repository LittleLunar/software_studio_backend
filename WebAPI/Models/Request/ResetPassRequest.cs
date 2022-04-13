
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class ResetPassRequest
{
  [Required]
  [JsonPropertyName("password")]
  public string Password { get; set; } = null!;

  [Required]
  [JsonPropertyName("confirm_password")]
  public string ConfirmPassword { get; set; } = null!;
}