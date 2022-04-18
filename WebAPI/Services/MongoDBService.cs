using software_studio_backend.Shared;
using software_studio_backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace software_studio_backend.Services;

public class MongoDBService
{
  public IMongoCollection<User> UserCollection { get; private set; }

  public IMongoCollection<Blog> BlogCollection { get; private set; }

  public IMongoCollection<Comment> CommentCollection { get; private set; }
  public MongoDBService(
    IOptions<DatabaseSettings> databaseSettings
  )
  {
    var mongoClient = new MongoClient(
      databaseSettings.Value.ConnectionString
    );

    Console.WriteLine("Connected To MongoDB");

    var mongoDatabase = mongoClient.GetDatabase(
      databaseSettings.Value.DatabaseName
    );

    UserCollection = mongoDatabase.GetCollection<User>(
      databaseSettings.Value.UsersCollectionName
    );

    BlogCollection = mongoDatabase.GetCollection<Blog>(
      databaseSettings.Value.BlogsCollectionName
    );


    CommentCollection = mongoDatabase.GetCollection<Comment>(
      databaseSettings.Value.CommentsCollectionName
    );
  }


}