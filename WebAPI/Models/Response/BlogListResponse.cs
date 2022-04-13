
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class BlogListResponse
{
  [JsonPropertyName("blogs")]
  public List<BlogResponse> Blogs { get; set; } = new List<BlogResponse>();
}