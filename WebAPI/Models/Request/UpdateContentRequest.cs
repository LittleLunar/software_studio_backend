
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class UpdateContentRequest
{
  [Required]
  [JsonPropertyName("update_content")]
  public string UpdatedContent { get; set; } = null!;
}