using Manage_Coffee.Models;
using Manage_Coffee.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Manage_Coffee.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public AccountRepository(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, IUserService userService, 
            IEmailService emailService, IConfiguration configuration) {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _emailService = emailService;
            _configuration = configuration;
        }
        public async Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel)
        {
            // Xóa confirm email khi tạo tài khoản
            var user = new ApplicationUser()
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Email = userModel.Email,
                UserName = userModel.Email,
            };
            var result = await _userManager.CreateAsync(user, userModel.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                if (!string.IsNullOrEmpty(token))
                {
                   
                    await SendEmailConfirmationEmail(user, token);
                    using (var context = new Cf2Context())
                    {
                        var khachHang = new KhachHang()
                        {
                            MaKh = userModel.Email,
                            Ten = $"{userModel.FirstName} {userModel.LastName}",
                            Email = userModel.Email,
                            Matkhau = userModel.Password,
                            Role = "Đồng"
                        };

                        // Thêm KhachHang vào cơ sở dữ liệu
                        context.KhachHangs.Add(khachHang);
                        await context.SaveChangesAsync(); // Lưu các thay đổi vào CSDL
                    }
                }
                else
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "Failed to generate email confirmation token."
                    });
                }

            }
            return result;
        }
        public async Task GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendEmailConfirmationEmail(user, token);
            }
        }
        public async Task GenerateForgotPasswordTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendForgotPasswordEmail(user, token);
            }
        }
        public async Task<IdentityResult> ConfirmEmailAsync(string uid, string token)
        {
            return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid), token);
        }
        public async Task<SignInResult> PasswordSignInAsync(SignInModel signInModel)
        {
            return await _signInManager.PasswordSignInAsync(signInModel.Email, signInModel.Password, signInModel.RememberMe, false);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model)
        {
            return await _userManager.ResetPasswordAsync(await _userManager.FindByIdAsync(model.UserId), model.Token, model.NewPassword);
        }
        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordModel model)
        {
            var userId = _userService.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Người dùng không tồn tại." });

            }
            return await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        }
        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        private async Task SendEmailConfirmationEmail(ApplicationUser user, string token)
        {
            //Thay thế appDomain bằng Localhost của mình trong file appsetting.json
            string appDomain = _configuration.GetSection("Application:AppDomain").Value;
            string confirmationLink = _configuration.GetSection("Application:EmailConfirmation").Value;
            if (user.Email != null)
            {
                UserEmailOptions options = new UserEmailOptions
                {
                    ToEmails = new List<string>() { user.Email },
                    PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.Email),
                    new KeyValuePair<string, string>("{{Link}}", string.Format(appDomain + confirmationLink, 
                    user.Id, token))
                }
                };
                await _emailService.SendEmailForEmailConfirmation(options);
            }
            else return;
        }
        private async Task SendForgotPasswordEmail(ApplicationUser user, string token)
        {
            string appDomain = _configuration.GetSection("Application:AppDomain").Value;
            string confirmationLink = _configuration.GetSection("Application:ForgotPassword").Value;

            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.FirstName),
                    new KeyValuePair<string, string>("{{Link}}",
                        string.Format(appDomain + confirmationLink, user.Id, token))
                }
            };

            await _emailService.SendEmailForForgotPassword(options);
        }
        /* =========================== */
        // Google Login
        public AuthenticationProperties ExternalLoginAsync(string provider, string redirectUrl)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return properties;
        }


        public async Task<IdentityResult> ExternalLoginCallbackAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return IdentityResult.Failed(new IdentityError { Description = "Thông tin đăng nhập bên ngoài không có sẵn." });

            // Kiểm tra nếu thông tin đăng nhập đã có trong bảng AspNetUserLogins (External Login)
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                return IdentityResult.Success;
            }

            // Kiểm tra xem email của người dùng đã tồn tại trong bảng AspNetUsers chưa
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
             
                var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                if (addLoginResult.Succeeded)
                {
                    // Update the EmailConfirmed field to true
                    if (!existingUser.EmailConfirmed)
                    {
                        existingUser.EmailConfirmed = true;
                        var updateResult = await _userManager.UpdateAsync(existingUser);
                        if (!updateResult.Succeeded)
                        {
                            return updateResult;
                        }
                    }
                    existingUser.EmailConfirmed = true;
                    // Sign in the user
                    await _signInManager.SignInAsync(existingUser, isPersistent: false);
                    return IdentityResult.Success;
                }
                return addLoginResult;
            }
            // Nếu email chưa tồn tại trong bảng AspNetUsers, tạo một tài khoản mới
            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? " ";
            var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? " ";
            var newUser = new ApplicationUser { UserName = email, Email = email, FirstName=firstName, LastName=lastName, EmailConfirmed = true};
            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                return createResult;
            }
            using (var context = new Cf2Context())
            {
                var khachHang = new KhachHang()
                {
                    MaKh = email,
                    Ten = $"{firstName} {lastName}",
                    Email = email,
                    Matkhau = email,
                    Role = "Đồng"
                };

                // Thêm KhachHang vào cơ sở dữ liệu
                context.KhachHangs.Add(khachHang);
                await context.SaveChangesAsync(); // Lưu các thay đổi vào CSDL
            }

            // Thêm thông tin đăng nhập từ Google vào tài khoản mới
            var addLoginToNewUserResult = await _userManager.AddLoginAsync(newUser, info);
            if (!addLoginToNewUserResult.Succeeded)
            {
                return addLoginToNewUserResult;
            }
            

            // Đăng nhập người dùng
            await _signInManager.SignInAsync(newUser, isPersistent: false);

            return IdentityResult.Success;
        }

    }
}
