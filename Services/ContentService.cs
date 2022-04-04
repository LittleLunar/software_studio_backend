using software_studio_backend.Models;
using MongoDB.Driver;

namespace software_studio_backend.Services;

public class ContentService
{
  private readonly IMongoCollection<Content> _contentCollection;

  public ContentService(MongoDBService mongoDBService)
  {
    _contentCollection = mongoDBService.ContentCollection;
  }

  public async Task<List<Content>> GetAsync() => await _contentCollection.Find(_ => true).ToListAsync();

  public async Task<Content> GetAsync(string id) => await _contentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

  public async Task CreateAsync(Content newContent) => await _contentCollection.InsertOneAsync(newContent);

  public async Task UpdateAsync(string id, Content updatedContent) => await _contentCollection.ReplaceOneAsync(x => x.Id == id, updatedContent);

  public async Task RemoveAsync(string id) => await _contentCollection.DeleteOneAsync(x => x.Id == id);
}