using FileSharingWebsite.Entities.Models;

namespace FileSharingWebsite.Data.Repositories
{
    public interface IUploadRepository
    {
        void NewUpload(Upload upload);
        Upload GetUpload(int id);
        Upload GetUploadByPath(string path);
        IEnumerable<Upload> GetUploadsByOwner(int ownerId);
        bool IsExist(string path);
        void DeleteUserUploads(int ownerId);
        void DeleteUpload(int id);
        void UpdateName(int id, string name);
        void UpdateDescription(int id, string description);
        void UpdateUpload(int id, Upload upload);
        void IncreaseCounter(string path);
    }
}
