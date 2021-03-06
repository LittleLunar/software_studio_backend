
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class CreateAnnounceRequest
{
  [Required]
  [JsonPropertyName("content")]
  public string Content { get; set; } = null!;

}