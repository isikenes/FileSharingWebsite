using FileSharingWebsite.Data.Context;
using FileSharingWebsite.Entities.Models;

namespace FileSharingWebsite.Data.Repositories
{
    public class UploadRepository : BaseRepository, IUploadRepository
    {
        public UploadRepository(DapperDBContext context) : base(context)
        {
        }

        public void DeleteUpload(int id)
        {
            string query = @"delete from dbo.Uploads where upload_id=@Id";
            Execute(query, new { Id = id });
        }

        public void DeleteUserUploads(int ownerId)
        {
            string query = @"delete from dbo.Uploads where owner_id=@Id";
            Execute(query, new { Id = ownerId });
        }

        public Upload GetUpload(int id)
        {
            string query = @"select upload_id as UploadId, file_name as FileName, description, owner_id as OwnerId, path, counter from dbo.Uploads 
                             where upload_id=@Id";
            return Query<Upload>(query, new { Id = id });
        }

        public Upload GetUploadByPath(string path)
        {
            string query = @"select upload_id as UploadId, file_name as FileName, description, owner_id as OwnerId, path, counter from dbo.Uploads 
                             where path=@Path";
            return Query<Upload>(query, new { Path = path });
        }

        public IEnumerable<Upload> GetUploadsByOwner(int ownerId)
        {
            string query = @"select upload_id as UploadId, file_name as FileName, description, owner_id as OwnerId, path, counter from dbo.Uploads 
                             where owner_id=@OwnerId";
            return ListQuery<Upload>(query, new { OwnerId = ownerId });
        }

        public void IncreaseCounter(string path)
        {
            string query = @"update dbo.Uploads set counter=counter+1 where path=@Path";
            Execute(query, new { Path = path });
        }

        public bool IsExist(string path)
        {
            string query = @"SELECT 1 FROM dbo.Uploads WHERE path = @Path";
            return Query<int?>(query, new { Path = path }) != null;
        }

        public void NewUpload(Upload upload)
        {
            string query = @"insert into dbo.Uploads (file_name, description, owner_id, path,counter) 
                             values (@FileName, @Description, @OwnerId, @Path,@Counter)";
            Execute(query, new
            {
                upload.FileName,
                upload.Description,
                upload.OwnerId,
                upload.Path,
                upload.Counter
            });
        }

        public void UpdateDescription(int id, string description)
        {
            string query = @"update dbo.Uploads set description=@Description where upload_id=@Id";
            Execute(query, new { Description = description, Id = id });
        }

        public void UpdateName(int id, string name)
        {
            string query = @"update dbo.Uploads set file_name =@Name where upload_id=@Id";
            Execute(query, new { Name = name, Id = id });
        }

        public void UpdateUpload(int id, Upload upload)
        {
            string query = @"update dbo.Uploads set file_name =@Name, description =@Description where upload_id=@Id";
            Execute(query, new { Name = upload.FileName, Description = upload.Description, Id = id });
        }
    }
}
