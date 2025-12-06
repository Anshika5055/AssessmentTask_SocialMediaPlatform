using Social_Media.Models;
using Social_Media.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Social_Media.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly PostService _postService;
    private readonly ContentModerationService _moderationService;

    public PostController(PostService postService, ContentModerationService moderationService)
    {
        _postService = postService;
        _moderationService = moderationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
    {
        var posts = await _postService.GetPostsAsync();
        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Post>> GetPost(int id)
    {
        var post = await _postService.GetPostByIdAsync(id);

        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost(Post post)
    {
        if (_moderationService.ContainsBannedWords(post.Content))
        {
            return BadRequest(new { message = "Post contains inappropriate content. Please remove banned words." });
        }

        await _postService.CreatePostAsync(post);
        return CreatedAtAction(nameof(GetPost), new { id = post.PostID }, post);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, Post post)
    {
        if (id != post.PostID)
        {
            return BadRequest();
        }

        if (_moderationService.ContainsBannedWords(post.Content))
        {
            return BadRequest(new { message = "Post contains inappropriate content. Please remove banned words." });
        }

        var result = await _postService.UpdatePostAsync(post);

        if (result == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var result = await _postService.DeletePostAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return NoContent();
    }
}