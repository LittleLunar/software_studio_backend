using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class Comment
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

  public string Detail { get; set; } = "";

  public ulong Like { get; set; } = 0;

  [BsonElement("content_id")]
  public ObjectId? ContentId { get; set; }

  [BsonElement("user_id")]
  public ObjectId? UserId { get; set; }

  [BsonElement("comment_id")]
  public ObjectId? CommentId { get; set; }

  [BsonElement("created_date")]
  public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}