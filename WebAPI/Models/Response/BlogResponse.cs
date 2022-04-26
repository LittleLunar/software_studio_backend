
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class BlogResponse
{
  [JsonPropertyName("blog_id")]
  public string BlogId { get; set; } = null!;

  [JsonPropertyName("author")]
  public UserResponse? Author { get; set; }

  [JsonPropertyName("topic")]
  public string Topic { get; set; } = null!;

  [JsonPropertyName("content")]
  public string Content { get; set; } = "";

  [JsonPropertyName("category")]
  public List<string> Category { get; set; } = new List<string>();

  [JsonPropertyName("like")]
  public int Like { get; set; }

  [JsonPropertyName("like_users")]
  public List<LikeUserResponse> LikeUsers { get; set; } = new List<LikeUserResponse>();

  [JsonPropertyName("comments")]
  public List<CommentResponse> Comments { get; set; } = new List<CommentResponse>();

  [JsonPropertyName("hide")]
  public bool Hide { get; set; }

  [JsonPropertyName("deleted")]
  public bool Deleted { get; set; }

  [JsonPropertyName("created_date")]
  public DateTime CreatedDate { get; set; }

  [JsonPropertyName("updated_date")]
  public DateTime UpdatedDate { get; set; }

  public BlogResponse(Blog blog, User? user, List<LikeUserResponse>? likeUserResponses, List<CommentResponse>? comments)
  {
    this.BlogId = blog.Id;
    if (user != null)
      this.Author = new UserResponse(user);
    this.Topic = blog.Topic;
    this.Content = blog.Detail;
    this.Category = blog.Category;
    this.Like = blog.Like.Count;
    if (likeUserResponses != null)
      this.LikeUsers = likeUserResponses;
    if (comments != null)
      this.Comments = comments;
    this.Hide = blog.Hide;
    this.Deleted = blog.Deleted;
    this.CreatedDate = blog.CreatedDate;
    this.UpdatedDate = blog.UpdatedDate;
  }
}