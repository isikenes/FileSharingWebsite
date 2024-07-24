using FileSharingWebsite.Entities.Models;
using FileSharingWebsite.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FileSharingWebsite.Mvc.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly HttpClient client;
        Uri baseAddress = new Uri("http://localhost:5001/api");

        public AccountController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
            client = clientFactory.CreateClient();
            client.BaseAddress = baseAddress;
        }

        public async Task<IActionResult> Profile()
        {
            int ownerId = -1;
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out ownerId);

            HttpResponseMessage response0 = await client.GetAsync($"{baseAddress}/users/id/{ownerId}");
            string data0 = await response0.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<User>(data0);

            ViewBag.Email = user.Email;

            HttpResponseMessage response = await client.GetAsync($"{baseAddress}/uploads/owner/{ownerId}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var uploads = JsonSerializer.Deserialize<List<Upload>>(data);
                return View(uploads);
            }

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ChangeEmail([FromForm] string newEmail)
        {
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int id);

            try
            {
                HttpResponseMessage response = await client.GetAsync($"{baseAddress}/users/id/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "Failed to retrieve user." });
                }

                string data = await response.Content.ReadAsStringAsync();

                var user = JsonSerializer.Deserialize<User>(data);

                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                user.Email = newEmail;

                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var updateResponse = await client.PutAsync($"{baseAddress}/users/{id}", content);

                if (updateResponse.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    var updateResponseContent = await updateResponse.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = "Failed to update email." });
                }
            }
            catch (JsonException)
            {
                return Json(new { success = false, message = "Error parsing JSON response." });
            }
            catch (HttpRequestException)
            {
                return Json(new { success = false, message = "HTTP request error." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while changing the email." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromForm] string oldPassword, string newPassword, string reNewPassword)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int id))
            {
                return Json(new { success = false, message = "User not found." });
            }

            User user = null;
            HttpResponseMessage response = await client.GetAsync($"{baseAddress}/users/id/{id}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                user = JsonSerializer.Deserialize<User>(data);
            }

            if (!PasswordHelper.VerifyPassword(oldPassword, user.Password))
            {
                return Json(new { success = false, message = "Old password is not correct!" });
            }

            if (!newPassword.Equals(reNewPassword))
            {
                return Json(new { success = false, message = "New passwords do not match!" });
            }

            user.Password = PasswordHelper.HashPassword(newPassword);
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync($"{baseAddress}/users/{id}", content);

            if (response2.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }

            return Json(new { success = true });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            int id = -1;
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out id);

            await client.DeleteAsync($"{baseAddress}/uploads/owner/{id}");

            await client.DeleteAsync($"{baseAddress}/users/{id}");

            return RedirectToAction("Logout", "Login");
        }
    }
}
