using software_studio_backend.Models;
using MongoDB.Driver;


namespace software_studio_backend.Services;

public class UserService
{
  private readonly IMongoCollection<User> _userCollection;

  public UserService(MongoDBService mongoDBService)
  {
    _userCollection = mongoDBService.UserCollection;
  }

  public async Task<List<User>> GetAsync() => await _userCollection.Find(_ => true).ToListAsync();

  public async Task<User?> GetAsync(string id) => await _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

  public async Task CreateAsync(User newUser) => await _userCollection.InsertOneAsync(newUser);

  public async Task UpdateAsync(string id, User updateUser) => await _userCollection.ReplaceOneAsync(x => x.Id == id, updateUser);

  public async Task RemoveAsync(string id) => await _userCollection.DeleteOneAsync(x => x.Id == id);
}
