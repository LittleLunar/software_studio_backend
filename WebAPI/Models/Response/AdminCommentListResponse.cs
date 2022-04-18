using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class AdminCommentListResponse
{
  [JsonPropertyName("comments")]
  public List<AdminCommentResponse> Comments { get; set; } = new List<AdminCommentResponse>();
}