using software_studio_backend.Models;
using software_studio_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
  private readonly CommentService _commentService;

  public CommentController(CommentService commentService)
  {
    _commentService = commentService;
  }

  [HttpGet]
  public async Task<List<Comment>> Get() => await _commentService.GetAsync();

  [HttpGet("{id:length(24)}")]
  public async Task<ActionResult<Comment>> Get(string id)
  {
    Comment? comment = await _commentService.GetAsync(id);

    if (comment is null)
    {
      return NotFound();
    }

    return comment;
  }

  [HttpPost]
  public async Task<IActionResult> Post(Comment newComment)
  {
    await _commentService.CreateAsync(newComment);

    return CreatedAtAction(nameof(Get), new { id = newComment.Id }, newComment);

  }

  [HttpPut("{id:length(24)}")]
  public async Task<IActionResult> Update(string id, Comment updatedComment)
  {
    Comment? comment = await _commentService.GetAsync(id);

    if (comment is null)
    {
      return NotFound();
    }

    updatedComment.Id = comment.Id;

    await _commentService.UpdateAsync(id, updatedComment);

    return NoContent();
  }

  [HttpDelete("{id:length(24)}")]
  public async Task<IActionResult> Delete(string id)
  {
    Comment? comment = await _commentService.GetAsync(id);

    if (comment is null)
    {
      return NotFound();
    }

    await _commentService.RemoveAsync(id);

    return NoContent();
  }
}