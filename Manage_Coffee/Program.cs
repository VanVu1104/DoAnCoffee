using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Manage_Coffee.Helpers;
using Manage_Coffee.Models;
using Manage_Coffee.Repository;
using Manage_Coffee.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();

//Google
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
}).AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
	options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
	options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
});
//.AddCookie()
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15); // Thời gian tồn tại của session
    options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});


// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký DbContext với DI container trước khi tạo app
builder.Services.AddDbContext<Cf2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký dịch vụ Identity với DI container trước khi tạo app
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<Cf2Context>().AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

    options.SignIn.RequireConfirmedEmail = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
    //options.Lockout.MaxFailedAccessAttempts = 3;
});

#if DEBUG

// Uncomment this code to disable client side validations.
//    .AddViewOptions(option =>
//{
//    option.HtmlHelperOptions.ClientValidationEnabled = false;
//});
#endif
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true; // Cookie chỉ có thể truy cập qua HTTP
    options.Cookie.IsEssential = true; // Cookie cần thiết cho ứng dụng
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.Configure<SMTPConfigModel>(builder.Configuration.GetSection("SMTPConfig"));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
builder.Services.AddScoped<IUserService, UserService>();
// Tạo app sau khi đã đăng ký tất cả dịch vụ
builder.Services.AddSingleton(x => new PaypalClient(
		builder.Configuration["PaypalOptions:AppId"],
		builder.Configuration["PaypalOptions:AppSecret"],
		builder.Configuration["PaypalOptions:Mode"],

		   x.GetRequiredService<IHttpContextAccessor>()
));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=AccountAdmin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

