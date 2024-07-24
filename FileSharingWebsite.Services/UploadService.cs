using FileSharingWebsite.Data.Repositories;
using FileSharingWebsite.Entities.Models;

namespace FileSharingWebsite.Services
{
    public class UploadService : IUploadService
    {
        private readonly IUploadRepository uploadRepository;

        public UploadService(IUploadRepository uploadRepository)
        {
            this.uploadRepository = uploadRepository;
        }

        public void DeleteUpload(int id)
        {
            uploadRepository.DeleteUpload(id);
        }

        public void DeleteUserUploads(int ownerId)
        {
            var uploads = GetUploadsByOwner(ownerId);
            foreach (Upload u in uploads)
            {
                string complete = string.Concat(u.Path, Path.GetExtension(u.FileName));

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", complete);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            uploadRepository.DeleteUserUploads(ownerId);
        }

        public Upload GetUpload(int id)
        {
            return uploadRepository.GetUpload(id);
        }

        public Upload GetUploadByPath(string path)
        {
            return uploadRepository.GetUploadByPath(path);
        }

        public IEnumerable<Upload> GetUploadsByOwner(int ownerId)
        {
            return uploadRepository.GetUploadsByOwner(ownerId);
        }

        public void IncreaseCounter(string path)
        {
            uploadRepository.IncreaseCounter(path);
        }

        public bool IsExist(string path)
        {
            return uploadRepository.IsExist(path);
        }

        public void NewUpload(Upload upload)
        {
            uploadRepository.NewUpload(upload);
        }

        public void UpdateDescription(int id, string description)
        {
            uploadRepository.UpdateDescription(id, description);
        }

        public void UpdateName(int id, string name)
        {
            uploadRepository.UpdateName(id, name);
        }

        public void UpdateUpload(int id, Upload upload)
        {
            uploadRepository.UpdateUpload(id, upload);
        }
    }
}
