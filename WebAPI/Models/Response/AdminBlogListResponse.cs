
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class AdminBlogListResponse
{
  [JsonPropertyName("blogs")]
  public List<AdminBlogResponse> Blogs { get; set; } = new List<AdminBlogResponse>();
}