using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class Comment
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

  [BsonElement("detail")]
  public string Detail { get; set; } = "";

  [BsonElement("like")]
  public List<string> Like { get; set; } = new List<string>();

  [BsonElement("delete")]
  public bool Delete { get; set; } = false;

  [BsonElement("content_id")]
  [BsonRepresentation(BsonType.ObjectId)]
  [JsonPropertyName("content_id")]
  public string ContentId { get; set; } = null!;

  [BsonElement("user_id")]
  [BsonRepresentation(BsonType.ObjectId)]
  [JsonPropertyName("user_id")]
  public string UserId { get; set; } = null!;

  [BsonElement("created_date")]
  public DateTime CreatedDate { get; set; } = DateTime.Now;

  [BsonElement("updated_date")]
  public DateTime UpdatedDate { get; set; } = DateTime.Now;
}