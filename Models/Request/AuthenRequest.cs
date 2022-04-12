using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace software_studio_backend.Models;

public class AuthenRequest
{
  [Required]
  [JsonPropertyName("username")]
  public string username { get; set; } = null!;

  [Required]
  [JsonPropertyName("password")]
  public string password { get; set; } = null!;
}