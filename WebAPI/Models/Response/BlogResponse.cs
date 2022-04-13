
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class BlogResponse
{
  [JsonPropertyName("blog_id")]
  public string BlogId { get; set; } = null!;

  [JsonPropertyName("author_id")]
  public string AuthorId { get; set; } = null!;

  [JsonPropertyName("author_displayname")]
  public string AuthorDisplayName { get; set; } = null!;

  [JsonPropertyName("author_image")]
  public string? AuthorPicture { get; set; }

  [JsonPropertyName("topic")]
  public string Topic { get; set; } = null!;

  [JsonPropertyName("content")]
  public string Content { get; set; } = "";

  [JsonPropertyName("category")]
  public List<string> Category { get; set; } = new List<string>();

  [JsonPropertyName("like")]
  public int Like { get; set; }

  [JsonPropertyName("like_users")]
  public List<string> LikeUsers { get; set; } = null!;

  [JsonPropertyName("created_date")]
  public DateTime CreatedDate { get; set; }

  [JsonPropertyName("comments")]
  public List<CommentResponse> Comments { get; set; } = new List<CommentResponse>();

  public BlogResponse(Blog blog, User user, List<CommentResponse>? comments = null)
  {
    this.BlogId = blog.Id;
    this.AuthorId = user.Id;
    this.AuthorDisplayName = user.Name;
    this.AuthorPicture = user.ProfileImage;
    this.Topic = blog.Topic;
    this.Content = blog.Detail;
    this.Category = blog.Category;
    this.Like = blog.Like.Count;
    this.LikeUsers = blog.Like;
    this.CreatedDate = blog.CreatedDate;
    this.Comments = comments ?? this.Comments;
  }
}