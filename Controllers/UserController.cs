using software_studio_backend.Models;
using software_studio_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
  private readonly UserService _userService;

  public UserController(UserService userService) => _userService = userService;

  [HttpGet]
  public async Task<List<User>> Get() => await _userService.GetAsync();

  [HttpGet("{id:length(24)}")]
  public async Task<ActionResult<User>> Get(string id)
  {
    User? user = await _userService.GetAsync(id);

    if (user is null)
    {
      return NotFound();
    }
    return user;
  }

  [HttpPost]
  public async Task<IActionResult> Post(User newUser)
  {
    await _userService.CreateAsync(newUser);

    return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);

  }

  [HttpPut("{id:length(24)}")]
  public async Task<IActionResult> Update(string id, User updatedUser)
  {
    User? user = await _userService.GetAsync(id);
    if (user is null)
    {
      return NotFound();
    }
    updatedUser.Id = user.Id;

    await _userService.UpdateAsync(id, updatedUser);

    return NoContent();

  }

  [HttpDelete("{id:length(24)}")]
  public async Task<IActionResult> Delete(string id)
  {
    User? user = await _userService.GetAsync(id);

    if (user is null)
    {
      return NotFound();
    }

    await _userService.RemoveAsync(id);

    return NoContent();
  }
}