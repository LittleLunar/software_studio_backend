
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class PantipResponse
{
  [JsonPropertyName("pantip_id")]
  public string PantipId { get; set; } = null!;

  [JsonPropertyName("author")]
  public UserResponse Author { get; set; } = null!;

  [JsonPropertyName("topic")]
  public string Topic { get; set; } = null!;

  [JsonPropertyName("content")]
  public string Content { get; set; } = null!;

  [JsonPropertyName("like")]
  public int Like { get; set; }

  [JsonPropertyName("like_users")]
  public List<string> LikeUsers { get; set; } = null!;

  [JsonPropertyName("created_date")]
  public DateTime CreatedDate { get; set; }

  [JsonPropertyName("updated_date")]
  public DateTime UpdatedDate { get; set; }

  [JsonPropertyName("comments")]
  public List<CommentResponse> Comments { get; set; } = new List<CommentResponse>();
  public PantipResponse(Pantip pantip, User user, List<CommentResponse>? comments = null)
  {
    this.PantipId = pantip.Id;
    this.Author = new UserResponse(user);
    this.Topic = pantip.Topic;
    this.Content = pantip.Detail;
    this.Like = pantip.Like.Count;
    this.LikeUsers = pantip.Like;
    this.CreatedDate = pantip.CreatedDate;
    this.UpdatedDate = pantip.UpdatedDate;
    this.Comments = comments ?? this.Comments;

  }
}