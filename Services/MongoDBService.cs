using software_studio_backend.Shared;
using software_studio_backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace software_studio_backend.Services;

public class MongoDBService
{
  public IMongoCollection<User> UserCollection { get; private set; }

  public IMongoCollection<Content> ContentCollection { get; private set; }

  public IMongoCollection<Comment> CommentCollection { get; private set; }
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

    UserCollection = mongoDatabase.GetCollection<User>(
      testDatabaseSettings.Value.UsersCollectionName
    );

    ContentCollection = mongoDatabase.GetCollection<Content>(
      testDatabaseSettings.Value.ContentsCollectionName
    );

    CommentCollection = mongoDatabase.GetCollection<Comment>(
      testDatabaseSettings.Value.CommentsCollectionName
    );
  }


}