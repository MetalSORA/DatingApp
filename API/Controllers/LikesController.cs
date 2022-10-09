namespace API.Controllers;

[Authorize]
public class LikesController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    
    public LikesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username){
        var sourceUserId = User.GetUserId();
        var likedUser = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);
        var sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);

        if(likedUser == null) return NotFound();

        if(sourceUser.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);
        if(userLike != null) return BadRequest("You already liked this user");

        userLike = new UserLike{
            SourceUserId = sourceUserId,
            LikedUserId = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);
        if(await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to like user");
    }
    //get the user who you liked
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetLikedUser([FromQuery]LikesParams likesParams){
        likesParams.UserId = User.GetUserId();
        var users = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
        return Ok(users);
    }

    [HttpDelete("delete-like/{id}")]
    public async Task<ActionResult> DeleteLike(int id){
        var sourceUserId = User.GetUserId();
        var unlikedUser = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
        var sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);

        if(unlikedUser == null) return NotFound();

        if(sourceUser.Id == id) return BadRequest("You cannot unlike yourself");

        var userUnlike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, unlikedUser.Id);
        if(userUnlike == null) return BadRequest("You already unliked this user");
        sourceUser.LikedUsers.Remove(userUnlike);
        if(await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to unlike user");
    }
}