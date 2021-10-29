using HRMS.Core.Entities.Master;
using HRMS.Core.Helpers.CommonCRUDHelper;
using HRMS.Services.Repository.GenericRepository;
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

        [HttpGet]
        public async Task<IActionResult> CreateDepartment()
        {
            Department model = new Department();
            model.Name = "XTXXX";
            model.Code = "Dept01";
            model.Description = "Description";

            var createModel = CrudHelper.CreateHelper<Department>(model);

            var response = await _IDepartmentRepository.CreateEntity(createModel    );
            return Json("");

        }
    }
}
