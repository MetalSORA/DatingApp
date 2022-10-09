

namespace API.Interfaces
{
    public interface IBlocksRepository
    {
        Task<UserBlock> GetUserBlock(int sourceUserId,int blockedUserId);
        Task<AppUser> GetBlockedUsers(int userId);
        Task<IEnumerable<BlockDto>> GetUserBlocks(string predicate, int userId);
    }
}