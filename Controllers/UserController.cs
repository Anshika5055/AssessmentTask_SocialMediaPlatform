using Microsoft.AspNetCore.Mvc;
using Social_Media.Models;
using Social_Media.Services;
using Microsoft.EntityFrameworkCore;

namespace Social_Media.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    
    private readonly UserService _userService;
    private readonly PostService _postService;
    private readonly LikeService _likeService;
    private readonly CommentService _commentService;

    public UserController(UserService userService, PostService postService, LikeService likeService, CommentService commentService)
    {
        _userService = userService;
        _postService = postService;
        _likeService = likeService;
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        await _userService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = user.UserID }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        if (id != user.UserID)
        {
            return BadRequest();
        }

        var result = await _userService.UpdateUserAsync(user);

        if (result == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("engagement")]
    public async Task<ActionResult> GetUserEngagementScores()
    {
        var users = await _userService.GetUsersAsync();
        var posts = await _postService.GetPostsAsync();
        var likes = await _likeService.GetLikesAsync();
        var comments = await _commentService.GetCommentsAsync();

        var engagementScores = new List<object>();

        foreach (var user in users)
        {
            var userPosts = posts.Where(p => p.UserID == user.UserID).Count();
            var userLikes = likes.Where(l => l.UserID == user.UserID).Count();
            var userComments = comments.Where(c => c.UserID == user.UserID).Count();

            var score = (userPosts * 5) + (userLikes * 2) + (userComments * 3);

            engagementScores.Add(new
            {
                userID = user.UserID,
                userName = user.UserName,
                posts = userPosts,
                likes = userLikes,
                comments = userComments,
                score = score
            });
        }

        var sortedScores = engagementScores.OrderByDescending(x => ((dynamic)x).score).ToList();

        return Ok(sortedScores);
    }
}