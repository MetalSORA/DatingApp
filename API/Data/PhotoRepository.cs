namespace API.Data;

public class PhotoRepository : IPhotoRepository
{
    private readonly DataContext _context;
    public PhotoRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Photo> GetPhotoById(int photoId)
    {
        return await _context.Photos
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(p => p.Id == photoId);
    }

    public async Task<IEnumerable<PhotoForApprovalDTO>> GetUnapprovedPhotos()
    {
        return await _context.Photos
            .IgnoreQueryFilters()
            .Where(p => p.IsApproved == false)
            .Select(u => new PhotoForApprovalDTO{
                PhotoId = u.Id,
                Username = u.AppUser.UserName,
                Url = u.Url,
                IsApproved = u.IsApproved
            }).ToListAsync();
    }

    public void RemovePhoto(Photo photo)
    {
        _context.Photos.Remove(photo);
    }
}