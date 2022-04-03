using software_studio_backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace software_studio_backend.Services;

public class MongoDBService
{
  private readonly IMongoCollection<User> _userCollection;

  private readonly IMongoCollection<Content> _contentCollection;

  private readonly IMongoCollection<Comment> _commentCollection;

  public MongoDBService(
    IOptions<TestDatabaseSettings> testDatabaseSettings
  )
  {
    var mongoClient = new MongoClient(
      testDatabaseSettings.Value.ConnectingString
    );

    var mongoDatabase = mongoClient.GetDatabase(
      testDatabaseSettings.Value.DatabaseName
    );

    _userCollection = mongoDatabase.GetCollection<User>(
      testDatabaseSettings.Value.UsersCollectionName
    );

    _contentCollection = mongoDatabase.GetCollection<Content>(
      testDatabaseSettings.Value.ContentsCollectionName
    );

    _commentCollection = mongoDatabase.GetCollection<Comment>(
      testDatabaseSettings.Value.CommentsCollectionName
    );
  }

  public IMongoCollection<User> GetUserCollection { get; } = null!;

  public IMongoCollection<Content> GetContentCollection { get; } = null!;

  public IMongoCollection<Comment> GetCommentCollection { get; } = null!;
}