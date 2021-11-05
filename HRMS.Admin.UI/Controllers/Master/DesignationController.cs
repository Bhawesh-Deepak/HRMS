using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Master;
using HRMS.Services.Repository.GenericRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Admin.UI.Controllers.Master
{
    public class DesignationController : Controller
    {
        private readonly IGenericRepository<Department, int> _IDepartmentRepository;
        private readonly IGenericRepository<Designation, int> _IDesignationRepository;
        private readonly ILogger _logger;

        public DesignationController(IGenericRepository<Department, int> departmentRepo,
            IGenericRepository<Designation, int> designationRepo, ILogger logger)
        {
            _IDepartmentRepository = departmentRepo;
            _IDesignationRepository = designationRepo;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

      
        private async Task PopulateViewBag()
        {
            var departmentResponse = await _IDepartmentRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

            if (departmentResponse.ResponseStatus == ResponseStatus.Success) 
                ViewBag.DepartmentList = departmentResponse.Entities;

            _logger.LogError(departmentResponse.Message);

        }
    }
}
