using Social_Media.Models;
using Social_Media.Services;
using Microsoft.AspNetCore.Mvc;

namespace Social_Media.Controllers;

[ApiController]
[Route("[controller]")]
public class LikeController : ControllerBase
{
    private readonly LikeService _likeService;

    public LikeController(LikeService likeService)
    {
        _likeService = likeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Like>>> GetLikes()
    {
        var likes = await _likeService.GetLikesAsync();
        return Ok(likes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Like>> GetLike(int id)
    {
        var like = await _likeService.GetLikeByIdAsync(id);

        if (like == null)
        {
            return NotFound();
        }

        return Ok(like);
    }

    [HttpGet("post/{postId}")]
    public async Task<ActionResult<IEnumerable<Like>>> GetLikesByPost(int postId)
    {
        var likes = await _likeService.GetLikesByPostIdAsync(postId);
        return Ok(likes);
    }

    [HttpPost]
    public async Task<ActionResult<Like>> CreateLike(Like like)
    {
        await _likeService.CreateLikeAsync(like);
        return CreatedAtAction(nameof(GetLike), new { id = like.LikeID }, like);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLike(int id)
    {
        var result = await _likeService.DeleteLikeAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return NoContent();
    }
}

