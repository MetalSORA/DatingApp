
namespace API.Controllers
{
    [Authorize]
    public class BlocksController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public BlocksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }

        //blocking a user
        [HttpPost("{username}")]
        public async Task<ActionResult> AddBlock(string username){
            var sourceUserId=User.GetUserId();
            var blockedUser=await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);

            var sourceUser=await _unitOfWork.BlocksRepository.GetBlockedUsers(sourceUserId);
            if(blockedUser == null) return NotFound();
            if(sourceUser.UserName == username) return BadRequest("You cannot block yourself");

            var userBlock = await _unitOfWork.BlocksRepository.GetUserBlock(sourceUserId, blockedUser.Id);
            if(userBlock != null) return BadRequest("You already blocked this user");

            userBlock = new UserBlock{
                SourceUserId=sourceUserId,
                BlockedUserId=blockedUser.Id
            };

            sourceUser.BlockedUsers.Add(userBlock);

            if( await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to block the user");
        }

        //check this method maybe the predicate doesn't have anything and that's why doesn't show the blocked users 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlockDto>>> GetUserBlocks([FromQuery] string predicate){
            var userID = User.GetUserId();
            var users = await _unitOfWork.BlocksRepository.GetUserBlocks(predicate, userID);
            return Ok(users);
        }

        [HttpDelete("remove-block/{id}")]
        public async Task<ActionResult> DeleteBlock(int id){
            var sourceUserId = User.GetUserId();
            var unblockedUser = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
            var sourceUser = await _unitOfWork.BlocksRepository.GetBlockedUsers(sourceUserId);

            if(unblockedUser == null) return NotFound();

            if(sourceUser.Id == id) return BadRequest("You cannot unblock yourself");
            var userUnblock = await _unitOfWork.BlocksRepository.GetUserBlock(sourceUserId, unblockedUser.Id);
            if(userUnblock == null) return BadRequest("You already unblocked this user");
            sourceUser.BlockedUsers.Remove(userUnblock);
            if(await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to unblock user");
        }
    }
}