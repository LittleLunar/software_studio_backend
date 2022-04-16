
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;
public class CommentResponse
{
  [JsonPropertyName("comment_id")]
  public string CommentId { get; set; } = null!;

  [JsonPropertyName("author")]
  public UserResponse Author { get; set; } = null!;

  [JsonPropertyName("comment")]
  public string Comment { get; set; } = null!;

  [JsonPropertyName("like")]
  public int Like { get; set; } = 0;

  [JsonPropertyName("like_users")]
  public List<string> LikeUser { get; set; } = new List<string>();

  [JsonPropertyName("created_date")]
  public DateTime CreatedDate { get; set; }

  [JsonPropertyName("updated_date")]
  public DateTime UpdatedDate { get; set; }
  public CommentResponse(Comment comment, User user)
  {
    this.CommentId = comment.Id;
    this.Author = new UserResponse(user);
    this.Comment = comment.Detail;
    this.Like = comment.Like.Count;
    this.LikeUser = comment.Like;
    this.CreatedDate = comment.CreatedDate;
    this.UpdatedDate = comment.UpdatedDate;
  }
}