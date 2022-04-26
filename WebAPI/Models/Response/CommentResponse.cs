
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;
public class CommentResponse
{
  [JsonPropertyName("comment_id")]
  public string CommentId { get; set; } = null!;

  [JsonPropertyName("author")]
  public UserResponse? Author { get; set; }

  [JsonPropertyName("comment")]
  public string Comment { get; set; } = null!;

  [JsonPropertyName("like")]
  public int Like { get; set; } = 0;

  [JsonPropertyName("like_users")]
  public List<LikeUserResponse> LikeUser { get; set; } = new List<LikeUserResponse>();

  [JsonPropertyName("created_date")]
  public DateTime CreatedDate { get; set; }

  [JsonPropertyName("updated_date")]
  public DateTime UpdatedDate { get; set; }
  public CommentResponse(Comment comment, User? user, List<LikeUserResponse>? likeUserResponses)
  {
    this.CommentId = comment.Id;
    if (user != null)
      this.Author = new UserResponse(user);
    this.Comment = comment.Detail;
    this.Like = comment.Like.Count;
    if (likeUserResponses != null)
      this.LikeUser = likeUserResponses;
    this.CreatedDate = comment.CreatedDate;
    this.UpdatedDate = comment.UpdatedDate;
  }
}