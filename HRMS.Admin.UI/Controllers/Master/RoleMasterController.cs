using HRMS.Admin.UI.Helpers;
using HRMS.Core.Entities.Master;
using HRMS.Core.Helpers.CommonHelper;
using HRMS.Services.Repository.GenericRepository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRMS.Admin.UI.Controllers.Master
{
    public class RoleMasterController : Controller
    {
        private readonly IGenericRepository<RoleMaster, int> _IRoleMasterRepository;

        public RoleMasterController(IGenericRepository<RoleMaster, int> roleMasterRepo)
        {
            _IRoleMasterRepository = roleMasterRepo;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HeaderTitle = PageHeader.HeaderSetting["RoleMasterIndex"];
            return await Task.Run(() => View(ViewHelper.GetViewPathDetails("RoleMaster", "RoleIndex")));
        }
    }
}
