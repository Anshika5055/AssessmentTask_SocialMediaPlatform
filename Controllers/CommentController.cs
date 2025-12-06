using Social_Media.Models;
using Social_Media.Services;
using Microsoft.AspNetCore.Mvc;

namespace Social_Media.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    private readonly CommentService _commentService;
    private readonly ContentModerationService _moderationService;

    public CommentController(CommentService commentService, ContentModerationService moderationService)
    {
        _commentService = commentService;
        _moderationService = moderationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
    {
        var comments = await _commentService.GetCommentsAsync();
        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Comment>> GetComment(int id)
    {
        var comment = await _commentService.GetCommentByIdAsync(id);

        if (comment == null)
        {
            return NotFound();
        }

        return Ok(comment);
    }

    [HttpGet("post/{postId}")]
    public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByPost(int postId)
    {
        var comments = await _commentService.GetCommentsByPostIdAsync(postId);
        return Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult<Comment>> CreateComment(Comment comment)
    {
        if (_moderationService.ContainsBannedWords(comment.Content))
        {
            return BadRequest(new { message = "Comment contains inappropriate content. Please remove banned words." });
        }

        await _commentService.CreateCommentAsync(comment);
        return CreatedAtAction(nameof(GetComment), new { id = comment.CommentID }, comment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, Comment comment)
    {
        if (id != comment.CommentID)
        {
            return BadRequest();
        }

        if (_moderationService.ContainsBannedWords(comment.Content))
        {
            return BadRequest(new { message = "Comment contains inappropriate content. Please remove banned words." });
        }

        var result = await _commentService.UpdateCommentAsync(comment);

        if (result == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var result = await _commentService.DeleteCommentAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return NoContent();
    }
}

