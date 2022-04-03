using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class User
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; }
  
  [BsonElement("username")]
  public string Username { get; set; }

  [BsonElement("password")]
  public string Password { get; set; }

  [BsonElement("name")]
  [JsonPropertyName("name")]
  public string Name { get; set; }

  [BsonElement("role")]
  [JsonPropertyName("role")]
  public string Role { get; set; }

  [BsonElement("profile_image")]
  [JsonPropertyName("profile_image")]
  public string ProfileImage { get; set; }

  [BsonElement("active")]
  public bool Active { get; set; }

  [BsonElement("banned")]
  public bool Banned { get; set; }

  [BsonElement("created_date")]
  public bool Created_date { get; set; }
}