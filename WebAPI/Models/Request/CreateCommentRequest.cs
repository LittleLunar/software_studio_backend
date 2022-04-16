
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class CreateCommentRequest
{
  [Required]
  [JsonPropertyName("content")]
  public string Content { get; set; } = null!;

  [Required]
  [JsonPropertyName("content_id")]
  public string ContentId { get; set; } = null!;
  
}