using Microsoft.AspNetCore.Mvc;
using software_studio_backend.Models;
using software_studio_backend.Services;
namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentController : ControllerBase
{
  private readonly ContentService _contentService;

  public ContentController(ContentService contentService)
  {
    _contentService = contentService;
  }

  [HttpGet]
  public async Task<List<Content>> Get() => await _contentService.GetAsync();

  [HttpGet("{id:length(24)}")]
  public async Task<ActionResult<Content>> Get(string id)
  {
    Content? content = await _contentService.GetAsync(id);

    if (content is null)
    {
      return NotFound();
    }
    return content;
  }

  [HttpPost]
  public async Task<IActionResult> Post(Content newContent)
  {
    await _contentService.CreateAsync(newContent);

    return CreatedAtAction(nameof(Get), new { id = newContent.Id }, newContent);
  
  }

  [HttpPut("{id:length(24)}")]
  public async Task<IActionResult> Update(string id, Content updatedContent)
  {
    Content? content = await _contentService.GetAsync(id);

    if(content is null)
    {
      return NotFound();
    }

    updatedContent.Id = content.Id;
    
    await _contentService.UpdateAsync(id, updatedContent);
    
    return NoContent();
    
  }

  [HttpDelete("{id:length(24)}")]
  public async Task<IActionResult> Delete(string id)
  {
    Content? content = await _contentService.GetAsync(id);

    if(content is null)
    {
      return NotFound();
    }

    await _contentService.RemoveAsync(id);

    return NoContent();
  }
}