using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class Announcement
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

  [BsonElement("topic")]
  public string Topic { get; set; } = null!;

  [BsonElement("detail")]
  public string Detail { get; set; } = "";

  [BsonElement("user_id")]
  [BsonRepresentation(BsonType.ObjectId)]
  [JsonPropertyName("user_id")]
  public string UserId { get; set; } = null!;

  [BsonElement("created_date")]
  public DateTime CreatedDate { get; set; } = DateTime.Now;

  [BsonElement("updated_date")]
  public DateTime UpdatedDate { get; set; } = DateTime.Now;
}