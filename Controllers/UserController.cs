using software_studio_backend.Models;
using software_studio_backend.Services;
using software_studio_backend.Utils;
using software_studio_backend.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
  private readonly UserService _userService;

  public UserController(UserService userService)
  {
    _userService = userService;
  }

  [HttpPost]
  [Route("authenticate")]
  public async Task<IActionResult> Authenticate([FromBody] AuthenRequest request)
  {
    User? user = await _userService.GetByUsernameAsync(request.username);

    if (user is null) return NotFound("User is not found");

    if (user.Password != request.password) return Unauthorized("Username or Password is incorrect.");

    // generateAccessToken(username)
    string accessToken = TokenUtils.GenerateAccessToken(user);

    // generateRefreshToken(username)
    string refreshToken = TokenUtils.GenerateRefreshToken(user);


    // set tokens to Cookie
    setTokenCookie(Constant.Name.AccessToken, accessToken, DateTime.Now.AddSeconds(Constant.Number.AccessTokenExpiresInSec));
    setTokenCookie(Constant.Name.RefreshToken, refreshToken, DateTime.Now.AddMonths(Constant.Number.RefreshTokenExpiresInMonths));

    return Ok("Authen success");
  }

  private void setTokenCookie(string name, string token, DateTime expires)
  {
    Response.Cookies.Append(name, token, new CookieOptions
    {
      HttpOnly = true,
      Expires = expires
    });
  }

  [HttpGet]
  public async Task<List<User>> Get() => await _userService.GetAsync();

  [HttpGet("{id:length(24)}")]
  public async Task<ActionResult<User>> Get(string id)
  {
    User? user = await _userService.GetAsync(id);

    if (user is null) return NotFound();

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

    if (user is null) return NotFound();

    updatedUser.Id = user.Id;

    await _userService.UpdateAsync(id, updatedUser);

    return NoContent();

  }

  [HttpDelete("{id:length(24)}")]
  public async Task<IActionResult> Delete(string id)
  {
    User? user = await _userService.GetAsync(id);

    if (user is null) return NotFound();

    Token? token = SessionCollection.GetSession(user.Username!);

    if (token != null) SessionCollection.Invalidate(token);

    await _userService.RemoveAsync(id);

    return NoContent();
  }
}