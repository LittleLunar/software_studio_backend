
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class EditContentRequest
{
  [JsonPropertyName("topic")]
  public string? Topic { get; set; }
  [JsonPropertyName("content")]
  public string? Content { get; set; }

  [JsonPropertyName("hide")]
  public bool? Hide { get; set; }
}