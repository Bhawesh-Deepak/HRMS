using HRMS.Core.Entities.LeadManagement;
using HRMS.Core.Helpers.CommonHelper;
using HRMS.Services.Repository.GenericRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers.Customer
{
    public class EmployeeLeadController : Controller
    {
        private readonly IGenericRepository<CustomerDetail, int> _ICustomerDetailRepository;
        private readonly IGenericRepository<CustomerLeadDetail, int> _ICustomerLeadRepository;
        public EmployeeLeadController(IGenericRepository<CustomerDetail, int> _customerDetailRepo,
            IGenericRepository<CustomerLeadDetail, int> customerLeadRepo)
        {
            _ICustomerDetailRepository = _customerDetailRepo;
            _ICustomerLeadRepository = customerLeadRepo;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.HeaderTitle = "Employee Lead";
            return await Task.Run(() => View(ViewHelper.GetViewPathDetails("EmployeeLead", "EmployeeLeadIndex")));
        }

        public async Task<IActionResult> CreateLead()
        {
            return await Task.Run(() => PartialView(ViewHelper.GetViewPathDetails("EmployeeLead", "EmployeeLeadCreate")));
        }
    }
}
