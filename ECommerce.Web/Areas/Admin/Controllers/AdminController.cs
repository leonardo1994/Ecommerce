using System.Web.Mvc;

namespace ECommerce.Web.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Home
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}