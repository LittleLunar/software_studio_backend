
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace software_studio_backend.Models;

public class CreateBlogRequest
{
  [Required]
  [JsonPropertyName("topic")]
  public string Topic { get; set; } = null!;

  [Required]
  [JsonPropertyName("content")]
  public string Content { get; set; } = null!;

  [JsonPropertyName("category")]
  public List<string> Category { get; set; } = new List<string>();
}