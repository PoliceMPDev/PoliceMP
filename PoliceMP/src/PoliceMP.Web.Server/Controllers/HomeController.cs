using Microsoft.AspNetCore.Mvc;
using PoliceMP.Core.Server.Communications.Interfaces;

namespace PoliceMP.Web.Server.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Json(new
            {
                Name = "PoliceMP"
            });
        }
    }
}