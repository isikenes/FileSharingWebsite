using FileSharingWebsite.Entities.Models;
using FileSharingWebsite.Helpers;
using HeyRed.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FileSharingWebsite.Mvc.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly HttpClient client;
        Uri baseAddress = new Uri("http://localhost:5001/api");

        public UploadController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
            client = clientFactory.CreateClient();
            client.BaseAddress = baseAddress;
        }

        [Route("/Upload/{path}")]
        public async Task<IActionResult> Index(string path)
        {
            HttpResponseMessage response = await client.GetAsync($"{baseAddress}/uploads/path/{path}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            string data = await response.Content.ReadAsStringAsync();
            Upload? upload = JsonSerializer.Deserialize<Upload>(data);

            if (upload == null)
            {
                return View("FileNotFound");
            }

            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int id);

            ViewBag.IsOwner = id == upload.OwnerId;

            return View(upload);
        }

        public IActionResult FileNotFound()
        {
            return View();
        }

        [Route("/Upload/New")]
        public IActionResult New()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [Route("/Upload/New")]
        [HttpPost]
        public async Task<IActionResult> New([FromForm] Upload upload, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                int maxContentLength = 1024 * 1024 * 100; //100 mb
                if (file.Length > maxContentLength)
                {
                    ModelState.AddModelError("", "File can not be larger than 100MB!");
                    return View();
                }

                int ownerId = -1;
                int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out ownerId);
                upload.OwnerId = ownerId;

                string fileName;
                bool fileExists = false;
                do
                {
                    fileName = FileHelper.GenerateRandomFilename();
                    var response0 = await client.GetAsync($"{baseAddress}/uploads/exists/{fileName}");

                    if (response0.IsSuccessStatusCode)
                    {
                        var responseContent = await response0.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                        fileExists = result.GetProperty("Exists").GetBoolean();

                    }
                    else
                    {
                        ModelState.AddModelError("", "Error checking file existence.");
                        return View();
                    }
                } while (fileExists);

                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", string.Concat(fileName, extension));
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                upload.Path = fileName;
                upload.FileName = string.Concat(upload.FileName, extension);

                var jsonContent = new StringContent(JsonSerializer.Serialize(upload), Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{baseAddress}/uploads", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<dynamic>(responseContent);
                    return RedirectToAction("Index", "Upload", new { path = fileName });
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<dynamic>(responseContent);
                    ModelState.AddModelError("", result.message.ToString());
                }
            }
            return View();
        }



        [Route("/Dowload/{file}")]
        public async Task<IActionResult> Download(string file, string name)
        {
            string complete = string.Concat(file, Path.GetExtension(name));
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", complete);

            if (!System.IO.File.Exists(filePath))
            {
                return RedirectToAction("FileNotFound");
            }

            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = name,
                Inline = false
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            var content = new StringContent(file, Encoding.UTF8, "application/json");
            await client.PutAsync($"{baseAddress}/uploads/inc/{file}", content);


            return File(System.IO.File.ReadAllBytes(filePath), MimeTypesMap.GetMimeType(name), name);
        }

        [ValidateAntiForgeryToken]
        [Route("Upload/Delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteUpload(int id)
        {
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId);

            HttpResponseMessage response = await client.GetAsync($"{baseAddress}/uploads/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            string data = await response.Content.ReadAsStringAsync();
            Upload upload = JsonSerializer.Deserialize<Upload>(data);

            if (userId == upload.OwnerId) //double check
            {

                string complete = string.Concat(upload.Path, Path.GetExtension(upload.FileName));

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", complete);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                await client.DeleteAsync($"{baseAddress}/uploads/{id}");
            }
            return RedirectToAction("Profile", "Account");
        }

        [ValidateAntiForgeryToken]
        [Route("Upload/UpdateName")]
        [HttpPost]
        public async Task<IActionResult> ChangeUploadName(int id, string newName)
        {
            int userId = -1;
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);

            HttpResponseMessage response = await client.GetAsync($"{baseAddress}/uploads/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            string data = await response.Content.ReadAsStringAsync();
            Upload upload = JsonSerializer.Deserialize<Upload>(data);

            if (userId == upload.OwnerId) //double check
            {
                newName = string.Concat(newName, Path.GetExtension(upload.FileName));
                upload.FileName = newName;

                var json = JsonSerializer.Serialize(upload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response2 = await client.PutAsync($"{baseAddress}/uploads/{id}", content);
            }
            return RedirectToAction("Index", new { path = upload.Path });
        }

        [ValidateAntiForgeryToken]
        [Route("Upload/UpdateDescription")]
        [HttpPost]
        public async Task<IActionResult> ChangeUploadDescription(int id, string newDescription)
        {
            int userId = -1;
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);

            HttpResponseMessage response = await client.GetAsync($"{baseAddress}/uploads/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }
            string data = await response.Content.ReadAsStringAsync();
            Upload upload = JsonSerializer.Deserialize<Upload>(data);

            if (userId == upload.OwnerId) //double check
            {
                upload.Description = newDescription;

                var json = JsonSerializer.Serialize(upload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response2 = await client.PutAsync($"{baseAddress}/uploads/{id}", content);
            }
            return RedirectToAction("Index", new { path = upload.Path });
        }
    }
}
