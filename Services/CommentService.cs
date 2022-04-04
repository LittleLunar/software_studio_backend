using software_studio_backend.Models;
using MongoDB.Driver;

namespace software_studio_backend.Services;

public class CommentService
{
  private readonly IMongoCollection<Comment> _commentCollection;

  public CommentService(MongoDBService mongoDBService)
  {
    _commentCollection = mongoDBService.CommentCollection;
  }

  public async Task<List<Comment>> GetAsync() => await _commentCollection.Find(_ => true).ToListAsync();

  public async Task<Comment> GetAsync(string id) => await _commentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

  public async Task CreateAsync(Comment newComment) => await _commentCollection.InsertOneAsync(newComment);

  public async Task UpdateAsync(string id, Comment updatedComment) => await _commentCollection.ReplaceOneAsync(x => x.Id == id, updatedComment);

  public async Task RemoveAsync(string id) => await _commentCollection.DeleteOneAsync(x => x.Id == id);
}