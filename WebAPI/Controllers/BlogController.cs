using Microsoft.AspNetCore.Mvc;
using software_studio_backend.Models;
using software_studio_backend.Services;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
  private readonly MongoDBService _mongoDB;

  public BlogController(MongoDBService mongoDBService)
  {
    _mongoDB = mongoDBService;
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("list")]
  public async Task<IActionResult> GetBlogs() // All blogs page
  {
    List<Blog> blogs = await _mongoDB.BlogCollection.Find(_ => true).ToListAsync();

    List<BlogResponse> blogResponses = new List<BlogResponse>();

    foreach (Blog blog in blogs)
    {
      User user = await _mongoDB.UserCollection.Find(x => x.Id == blog.UserId).FirstAsync();
      BlogResponse blogResponse = new BlogResponse(blog, user);
      blogResponses.Add(blogResponse);
    }

    BlogListResponse blogList = new BlogListResponse { Blogs = blogResponses.OrderByDescending(x => x.CreatedDate).ToList() };

    return Ok(blogList);
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("{id:length(24)}")]

  public async Task<IActionResult> GetBlog(string id) // Individual blog page with its comments.
  {
    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound(new ErrorMessage("Blog is not found"));

    User? author = await _mongoDB.UserCollection.Find(x => x.Id == blog.UserId).FirstOrDefaultAsync();

    if (author == null)
      return NotFound(new ErrorMessage("Author is not found"));

    BlogResponse blogResponse = new BlogResponse(blog, author);

    return Ok(blog);

  }

  [Authorize(Roles = "admin")]
  [HttpPost]
  [Route("create")]
  public async Task<IActionResult> CreateBlog([FromBody] CreateBlogRequest body)
  {
    string? username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    if (String.IsNullOrEmpty(username))
      return Unauthorized(new ErrorMessage("You are not authorized user."));

    User user = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstAsync();

    Blog newBlog = new Blog { Topic = body.Topic, Category = body.Category, Detail = body.Content, UserId = user.Id };

    await _mongoDB.BlogCollection.InsertOneAsync(newBlog);

    return CreatedAtAction(nameof(CreateBlog), newBlog);

  }

  [Authorize(Roles = "admin")]
  [HttpPatch]
  [Route("update/{id:length(24)}")]
  public async Task<IActionResult> UpdateBlog(string id, [FromBody] EditContentRequest body)
  {
    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound("Blog is not found.");

    blog.Detail = body.Content;
    blog.UpdatedDate = DateTime.Now;

    await _mongoDB.BlogCollection.ReplaceOneAsync(x => x.Id == id, blog);

    return Ok(blog);
  }

  [Authorize]
  [HttpPatch]
  [Route("like/{id:length(24)}")]
  public async Task<IActionResult> LikeBlog(string id)
  {
    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound(new ErrorMessage("Blog is not found."));

    string? username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    if (String.IsNullOrEmpty(username))
      return Unauthorized(new ErrorMessage("You are not authorized user."));

    if (blog.Like.Contains(username))
      blog.Like.Remove(username);
    else
      blog.Like.Add(username);

    await _mongoDB.BlogCollection.ReplaceOneAsync(x => x.Id == id, blog);

    return Ok(blog);

  }

  [Authorize(Roles = "admin")]
  [HttpDelete]
  [Route("delete/{id:length(24)}")]
  public async Task<IActionResult> DeleteBlog(string id)
  {
    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (blog is null)
      return NotFound(new ErrorMessage("Blog is not Found."));

    await _mongoDB.BlogCollection.DeleteOneAsync(x => x.Id == id);

    return NoContent();
  }

}