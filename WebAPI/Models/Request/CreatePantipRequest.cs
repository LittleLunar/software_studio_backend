
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace software_studio_backend.Models;

public class CreatePantipRequest
{
  [Required]
  [JsonPropertyName("topic")]
  public string Topic { get; set; } = null!;

  [Required]
  [JsonPropertyName("content")]
  public string Content { get; set; } = null!;

}