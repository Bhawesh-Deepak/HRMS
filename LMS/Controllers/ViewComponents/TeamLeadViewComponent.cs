using HRMS.Core.Entities.Payroll;
using HRMS.Services.Repository.GenericRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers.ViewComponents
{
    public class TeamLeadViewComponent : ViewComponent
    {
        private readonly IGenericRepository<EmployeeDetail, int> _IEmployeeDetailRepository;
      
        public TeamLeadViewComponent(IGenericRepository<EmployeeDetail, int> employeeDetailRepo)
        {
            _IEmployeeDetailRepository = employeeDetailRepo;
           
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var empCode = HttpContext.Session.GetString("empCode");
            var EmployeeDetailList = await _IEmployeeDetailRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            var EmployeeTeamLeader = EmployeeDetailList.Entities.Where(x => x.SuperVisorCode == empCode).ToList();
            return await Task.FromResult((IViewComponentResult)View("_TeamLeadDetail",EmployeeTeamLeader));
        }
    }
}
