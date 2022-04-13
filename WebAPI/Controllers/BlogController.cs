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

    BlogListResponse blogList = new BlogListResponse { Blogs = blogResponses };

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

    User author = await _mongoDB.UserCollection.Find(x => x.Id == blog.UserId).FirstAsync();

    if (author == null)
      return NotFound(new ErrorMessage("Author is not found"));

    List<CommentResponse> comments = new List<CommentResponse>();

    List<Comment> commentsInBlog = await _mongoDB.CommentCollection.Find(x => x.ContentId == blog.Id).ToListAsync();

    foreach (Comment comment in commentsInBlog)
    {
      User user = await _mongoDB.UserCollection.Find(x => x.Id == comment.UserId).FirstAsync();

      comments.Add(new CommentResponse(comment, user));
    }

    BlogResponse blogResponse = new BlogResponse(blog, author, comments);

    return Ok(blog);

  }

  [Authorize(Roles = "admin")]
  [HttpPost]
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
  [Route("{id:length(24)}/update")]
  public async Task<IActionResult> UpdateBlog(string id, [FromBody] UpdateContentRequest body)
  {
    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound("Blog is not found.");

    blog.Detail = body.UpdatedContent;
    blog.UpdatedDate = DateTime.Now;

    await _mongoDB.BlogCollection.ReplaceOneAsync(x => x.Id == id, blog);

    return Ok(blog);
  }

  [Authorize]
  [HttpPatch]
  [Route("{id:length(24)}/like")]
  public async Task<IActionResult> LikeBlog(string id)
  {
    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound("Blog is not found.");

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
  [Route("{id:length(24)}")]
  public async Task<IActionResult> DeleteBlog(string id)
  {
    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (blog is null)
      return NotFound(new ErrorMessage("Blog is not Found."));

    await _mongoDB.BlogCollection.DeleteOneAsync(x => x.Id == id);
    await _mongoDB.CommentCollection.DeleteManyAsync(x => x.ContentId == id);

    return NoContent();
  }

}