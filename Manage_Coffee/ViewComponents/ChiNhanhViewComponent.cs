using Manage_Coffee.Models;
using Microsoft.AspNetCore.Mvc;

namespace Manage_Coffee.ViewComponents
{
	public class ChiNhanhViewComponent : ViewComponent
	{
		private readonly Cf2Context _context;

		public ChiNhanhViewComponent(Cf2Context context)
		{
			_context = context;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var cn = await Task.Run(() => _context.ChiNhanhs.ToList());
			return View(cn);
		}
	}
}
