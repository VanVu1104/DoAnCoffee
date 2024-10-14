using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Manage_Coffee.Helpers
{
	public class SessionOrAuthorizeAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var sessionUserName = context.HttpContext.Session.GetString("UserName");
			var isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;

			// Kiểm tra nếu session không có thông tin người dùng và người dùng chưa được xác thực
			if (string.IsNullOrEmpty(sessionUserName) && !isAuthenticated)
			{
				// Redirect về trang đăng nhập nếu không có session hoặc xác thực
				context.Result = new RedirectToActionResult("Login", "DKDN", null);
			}

			base.OnActionExecuting(context);
		}
	}
}
