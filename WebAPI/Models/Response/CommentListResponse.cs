using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class CommentListResponse
{
  [JsonPropertyName("comments")]
  public List<CommentResponse> Comments { get; set; } = new List<CommentResponse>();
}