using Microsoft.AspNetCore.Mvc;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Shared.Models;

namespace PoliceMP.Web.Server.Controllers
{
    [Route("api/[controller]")]
    public class PedInfoController : Controller
    {
        private readonly IPedInfoService _pedInfoService;

        public PedInfoController(IPedInfoService pedInfoService)
        {
            _pedInfoService = pedInfoService;
        }

        [HttpGet("GetByNetworkId/{id}")]
        public ActionResult<PedInfo> GetByNetworkId(int id)
        {
            var pedInfo = _pedInfoService.GetByNetworkId(id);
            if (pedInfo == null) return NotFound();
            return pedInfo;
        }

        [HttpGet("GetByName/{name}")]
        public ActionResult<PedInfo> GetByName(string name)
        {
            var pedInfo = _pedInfoService.GetByName(name);
            if (pedInfo == null) return NotFound();
            return pedInfo;
        }
    }
}