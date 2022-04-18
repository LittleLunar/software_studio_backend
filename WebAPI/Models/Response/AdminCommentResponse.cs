
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;
public class AdminCommentResponse
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

  [JsonPropertyName("blog_id")]
  public string BlogId { get; set; } = null!;

  [JsonPropertyName("deleted")]
  public bool Deleted { get; set; }

  [JsonPropertyName("created_date")]
  public DateTime CreatedDate { get; set; }

  [JsonPropertyName("updated_date")]
  public DateTime UpdatedDate { get; set; }
  public AdminCommentResponse(Comment comment, User user)
  {
    this.CommentId = comment.Id;
    this.Author = new UserResponse(user);
    this.Comment = comment.Detail;
    this.Like = comment.Like.Count;
    this.LikeUser = comment.Like;
    this.BlogId = comment.ContentId;
    this.Deleted = comment.Delete;
    this.CreatedDate = comment.CreatedDate;
    this.UpdatedDate = comment.UpdatedDate;
  }
}