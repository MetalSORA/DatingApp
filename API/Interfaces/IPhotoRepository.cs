namespace API.Interfaces;

public interface IPhotoRepository
{
    Task<IEnumerable<PhotoForApprovalDTO>> GetUnapprovedPhotos();
    Task<Photo> GetPhotoById(int photoId);
    void RemovePhoto(Photo photo);
}