using FileSharingWebsite.Entities.Models;

namespace FileSharingWebsite.Services
{
    public interface IUploadService
    {
        Upload GetUpload(int id);
        Upload GetUploadByPath(string path);
        IEnumerable<Upload> GetUploadsByOwner(int ownerId);
        void NewUpload(Upload upload);
        bool IsExist(string path);
        void DeleteUserUploads(int ownerId);
        void DeleteUpload(int id);
        void UpdateName(int id, string name);
        void UpdateDescription(int id, string description);
        void UpdateUpload(int id, Upload upload);
        void IncreaseCounter(string path);
    }
}
