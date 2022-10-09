
namespace API.Data
{
    public class BlocksRepository : IBlocksRepository
    {
        private readonly DataContext _context;
        public BlocksRepository(DataContext context)
        {
            _context = context;
        }

        //getting all the blocked users
        public async Task<AppUser> GetBlockedUsers(int userId)
        {
            return await _context.Users
                .Include(x => x.BlockedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<UserBlock> GetUserBlock(int sourceUserId, int blockedUserId)
        {
            return await _context.BlockedUsers.FindAsync(sourceUserId, blockedUserId);
        }


        //I need to check if the if is needed or not
        public async Task<IEnumerable<BlockDto>> GetUserBlocks(string predicate,int userId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var blocks = _context.BlockedUsers.AsQueryable();

            if(predicate == "blocked"){
                blocks = blocks.Where(block => block.SourceUserId == userId);
                users = blocks.Select(block => block.BlockedUser);
            }

            return await users.Select(user => new BlockDto{
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }
    }
}