using DNTCaptcha.Core;
using FileSharingWebsite.Entities.Models;
using FileSharingWebsite.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FileSharingWebsite.Mvc.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly HttpClient client;
        Uri baseAdress = new Uri("http://localhost:5001/api");
        public LoginController(IHttpClientFactory httpClientFactory)
        {
            clientFactory = httpClientFactory;
            client = clientFactory.CreateClient();
            client.BaseAddress = baseAdress;
        }

        [Route("/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("/Login")]
        [ValidateAntiForgeryToken]
        [ValidateDNTCaptcha(ErrorMessage = "Captcha answer is not correct")]
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var content = new StringContent(JsonSerializer.Serialize(new LoginRequest { Email = email, Password = password }), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(baseAdress + "/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent);
                Console.WriteLine(responseContent);
                if (loginResponse.IsSuccess)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, loginResponse.UserId.ToString()),
                        new Claim(ClaimTypes.Email, email)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        [Route("/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Route("/Signup")]
        public IActionResult SignUp()
        {
            return View();
        }

        [Route("/Signup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateDNTCaptcha(ErrorMessage = "Captcha answer is not correct")]
        public async Task<IActionResult> SignUp(string email, string password, string rePassword)
        {
            User user = null;
            HttpResponseMessage response = await client.GetAsync($"{baseAdress}/users/{email}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                user = JsonSerializer.Deserialize<User>(data);
            }


            if (user != null)
            {
                return Json(new { success = false, message = "This email address is already registered!" });
            }
            if (!password.Equals(rePassword))
            {
                return Json(new { success = false, message = "Passwords do not match!" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data!" });
            }

            TempData["VerificationCode"] = EmailCodeHelper.SendVerificationCode(email);
            TempData["Email"] = email;
            TempData["Password"] = password;

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/VerifyCode")]
        public async Task<IActionResult> VerifyCode(string code)
        {
            string storedCode = TempData["VerificationCode"]?.ToString();
            string email = TempData["Email"]?.ToString();
            string password = TempData["Password"]?.ToString();


            if (code == storedCode && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                User newUser = new User();
                newUser.Email = email;
                newUser.Password = PasswordHelper.HashPassword(password);

                var json = JsonSerializer.Serialize(newUser);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/users", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData.Clear();
                    return Json(new { success = true });
                }

            }

            return Json(new { success = false });
        }


        [Route("/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("/ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Route("/ForgotPassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string email)
        {
            TempData["RecoveryCode"] = EmailCodeHelper.SendRecoveryCode(email);
            TempData["Email"] = email;
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/RecoveryCode")]
        public IActionResult RecoveryCode(string code)
        {
            string storedCode = TempData["RecoveryCode"]?.ToString();

            if (code == storedCode)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [Route("/ResetPassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string newPassword)
        {
            if (ModelState.IsValid)
            {
                string email = TempData["Email"]?.ToString();

                User user = null;
                HttpResponseMessage response = await client.GetAsync($"{baseAdress}/users/email/{email}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    user = JsonSerializer.Deserialize<User>(data);

                    user.Password = PasswordHelper.HashPassword(newPassword);

                    var json = JsonSerializer.Serialize(user);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response2 = await client.PutAsync("api/users", content);

                    if (response2.IsSuccessStatusCode)
                    {
                        TempData.Clear();
                        return Json(new { success = true });
                    }
                }

                return RedirectToAction("Login", "Login");
            }

            return View();
        }
    }
}
