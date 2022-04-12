using software_studio_backend.Models;
using software_studio_backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
  private readonly MongoDBService _mongoDB;

  public UserController(MongoDBService mongoDBService)
  {
    _mongoDB = mongoDBService;
  }



  [HttpGet]
  public async Task<IActionResult> Get()
  {
    List<User> users = await _mongoDB.UserCollection.Find(_ => true).ToListAsync();

    return Ok(users.OrderByDescending(x => x.Created_date).ToList());
  }

  [HttpGet("{id:length(24)}")]
  public async Task<IActionResult> Get(string id)
  {
    User? user = await _mongoDB.UserCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (user is null) return NotFound();

    return Ok(user);
  }

  [HttpPut("{id:length(24)}")]
  public async Task<IActionResult> Update(string id, User updatedUser)
  {

    User? user = await _mongoDB.UserCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (user is null) return NotFound();

    updatedUser.Id = user.Id;

    await _mongoDB.UserCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    return NoContent();

  }

  [HttpDelete("{id:length(24)}")]
  public async Task<IActionResult> Delete(string id)
  {
    User? user = await _mongoDB.UserCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (user is null) return NotFound();

    await _mongoDB.UserCollection.DeleteOneAsync(x => x.Id == id);

    return NoContent();
  }

  [HttpDelete]
  public async Task<IActionResult> Delete()
  {

    await _mongoDB.UserCollection.DeleteManyAsync(_ => true);

    return NoContent();
  }
}