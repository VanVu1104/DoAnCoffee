using Manage_Coffee.Models;
using Manage_Coffee.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Manage_Coffee.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository) {
            _accountRepository = accountRepository;
        }

        // Đăng nhập bằng Google
        [Route("login-google")]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = _accountRepository.ExternalLoginAsync(GoogleDefaults.AuthenticationScheme, redirectUrl);
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

		// Xử lý phản hồi sau khi đăng nhập với Google
		[Route("google-response")]
		public async Task<IActionResult> GoogleResponse()
		{
			// Kiểm tra kết quả đăng nhập
			var result = await _accountRepository.ExternalLoginCallbackAsync();
			if (result.Succeeded)
			{
				// Lấy email từ các claim của người dùng
				var emailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email");
				var email = emailClaim?.Value;

				if (!string.IsNullOrEmpty(email))
				{
					// Xử lý tiếp theo với email, ví dụ tìm khách hàng trong cơ sở dữ liệu hoặc tạo mới
					var user = await _accountRepository.GetUserByEmailAsync(email);
					if (user != null)
					{
						// User đã tồn tại, tiếp tục xử lý
						return RedirectToAction("Index", "Home");
					}
					else
					{
						// Nếu chưa có user, bạn có thể tạo mới hoặc yêu cầu thêm thông tin
						return RedirectToAction("SignUp", "Account");
					}
				}
				else
				{
					ModelState.AddModelError("", "Không thể lấy email từ Google");
					return RedirectToAction("Login");
				}
			}

			return RedirectToAction("Login");
		}



		/* ======================================================= */
		/* Login with Gmail */
		[Route("signup")]
        public IActionResult Signup()
        {
            return View();
        }

        [Route("signup")]
        [HttpPost]
        public async Task<ActionResult> Signup(SignUpUserModel userModel)
        {
            if (ModelState.IsValid) {
                var result = await _accountRepository.CreateUserAsync(userModel);
                if (!result.Succeeded) {
                    foreach (var errorMessage in result.Errors) {
                        ModelState.AddModelError("", errorMessage.Description);
                    }
                    return View(userModel);
                }
                ModelState.Clear();
            }
            return View(userModel);
        }

        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult> Login(SignInModel signInModel)
        {
            if (ModelState.IsValid) {
                var result = await _accountRepository.PasswordSignInAsync(signInModel);
                if (result.Succeeded) { 
                    return RedirectToAction("Index","Home");
                }
                ModelState.AddModelError("", "Invaild credentials");
            }
            return View(signInModel);
        }
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountRepository.SignOutAsync();
            return RedirectToAction("Index","Home");
        } 
        [Route("change-password")]
        public IActionResult ChangePassword()
        {
            return View();
        } 
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid) {
                ViewBag.IsSuccess = true;
                var result = await _accountRepository.ChangePasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    return View();
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string uid, string token, string email)
        {
            EmailConfirmModel model = new EmailConfirmModel
            {
                Email = email
            };

            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                token = token.Replace(' ', '+');
                var result = await _accountRepository.ConfirmEmailAsync(uid, token);
                if (result.Succeeded)
                {
                    model.EmailVerified = true;
                }
            }

            return View(model);
        }
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(EmailConfirmModel model)
        {
            var user = await _accountRepository.GetUserByEmailAsync(model.Email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    model.EmailVerified = true;
                    return View(model);
                }

                await _accountRepository.GenerateEmailConfirmationTokenAsync(user);
                model.EmailSent = true;
                ModelState.Clear();
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong.");
            }
            return View(model);
        }
        [AllowAnonymous, HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        } 
        [AllowAnonymous, HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid) 
            {
                var user = await _accountRepository.GetUserByEmailAsync(model.Email);
                if (user != null)
                {
                    await _accountRepository.GenerateForgotPasswordTokenAsync(user);
                }

                ModelState.Clear();
                model.EmailSent = true;
            }
            return View(model);
        }
        [AllowAnonymous, HttpGet("reset-password")]
        public IActionResult ResetPassword(string uid, string token)
        {
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel
            {
                Token = token,
                UserId = uid
            };
            return View(resetPasswordModel);
        }

        [AllowAnonymous, HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                model.Token = model.Token.Replace(' ', '+');
                var result = await _accountRepository.ResetPasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    model.IsSuccess = true;
                    return View(model);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
    }
}
