using HRMS.Core.Entities.Master;
using HRMS.Core.Helpers.CommonCRUDHelper;
using HRMS.Services.Repository.GenericRepository;
using HRMS.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.UI.Controllers.Master
{
    public class DepartmentController : Controller
    {
        private readonly IGenericRepository<Department, int> _IDepartmentRepository;

        public DepartmentController(IGenericRepository<Department, int> departmentRepo)
        {
            _IDepartmentRepository = departmentRepo;
        }
        public async Task<IActionResult> DepartmentIndex()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment(Department model)
        {
            var response = await _IDepartmentRepository.CreateEntity(CrudHelper.CreateHelper<Department>(model));

            return Json(ResponseMessageHelper.GetResponseMessage(response.ResponseStatus,
                nameof(Department), nameof(CreateDepartment)));

        }

        [HttpGet]
        public async Task<IActionResult> GetDepartmentList()
        {
            var dbResponse = await _IDepartmentRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

            var response = ResponseMessageHelper.GetResponseMessage(dbResponse.ResponseStatus, nameof(Department), nameof(GetDepartmentList));

            if (response.isSuccess)
            {

            }

            return Json("");

        }
    }
}
