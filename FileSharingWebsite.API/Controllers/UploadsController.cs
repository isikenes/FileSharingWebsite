using FileSharingWebsite.Entities.Models;
using FileSharingWebsite.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileSharingWebsite.API.Controllers
{
    [Route("api/uploads")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly IUploadService uploadService;

        public UploadsController(IUploadService uploadService)
        {
            this.uploadService = uploadService;
        }

        [HttpGet("{id}")]
        public IActionResult GetUpload(int id)
        {
            Upload upload = uploadService.GetUpload(id);
            if (upload == null)
            {
                return NotFound();
            }

            return Ok(upload);
        }

        [HttpGet("path/{path}")]
        public IActionResult GetUploadByPath(string path)
        {
            Upload upload = uploadService.GetUploadByPath(path);
            if (upload == null)
            {
                return NotFound();
            }

            return Ok(upload);
        }

        [HttpGet("owner/{id}")]
        public IActionResult GetOwnerUploads(int id)
        {
            IEnumerable<Upload> uploads = uploadService.GetUploadsByOwner(id);

            return Ok(uploads);
        }

        [HttpDelete("owner/{id}")]
        public IActionResult DeleteOwnerUploads(int id)
        {
            uploadService.DeleteUserUploads(id);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAnUpload(int id)
        {
            uploadService.DeleteUpload(id);
            return Ok();
        }

        [HttpPost]
        public IActionResult NewUpload(Upload upload)
        {
            uploadService.NewUpload(upload);
            return Ok(new { success = true });

        }

        [HttpGet("exists/{fileName}")]
        public IActionResult IsFileNameExists(string fileName)
        {
            var Exists = uploadService.IsExist(fileName);
            return Ok(new { Exists });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUpload(int id, Upload upload)
        {
            var existingUpload = uploadService.GetUpload(id);
            if (existingUpload == null)
            {
                return NotFound();
            }

            existingUpload.FileName = upload.FileName;
            existingUpload.Description = upload.Description;

            uploadService.UpdateUpload(id, existingUpload);

            return Ok(new { success = true });
        }

        [HttpPut("inc/{path}")]
        public IActionResult UpdateUpload(string path)
        {
            uploadService.IncreaseCounter(path);

            return Ok(new { success = true });
        }
    }
}
