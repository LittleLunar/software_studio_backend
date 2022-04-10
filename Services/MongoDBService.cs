using software_studio_backend.Shared;
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
      testDatabaseSettings.Value.ConnectionString
    );

    Console.WriteLine("Connected To MongoDB");

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

  public IMongoCollection<User> UserCollection { get { return _userCollection; } }

  public IMongoCollection<Content> ContentCollection { get { return _contentCollection; } }

  public IMongoCollection<Comment> CommentCollection { get { return _commentCollection; } }

}