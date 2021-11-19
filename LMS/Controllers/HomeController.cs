using HRMS.Core.Entities.LeadManagement;
using HRMS.Core.Helpers.CommonHelper;
using HRMS.Core.ReqRespVm.Response.Leads;
using HRMS.Services.Repository.GenericRepository;
using LMS.Helper;
using LMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [CustomAuthenticate]
    public class HomeController : Controller
    {
        private readonly IGenericRepository<CustomerDetail, int> _ICustomerDetailRepository;
        private readonly IGenericRepository<CustomerLeadDetail, int> _ICustomerLeadRepository;
        public HomeController(IGenericRepository<CustomerDetail, int> iCustomerDetailRepository,
              IGenericRepository<CustomerLeadDetail, int> customerLeadRepo)
        {
            _ICustomerDetailRepository = iCustomerDetailRepository;
            _ICustomerLeadRepository = customerLeadRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Error()
        {
            return await Task.Run(()=> View(ViewHelper.GetViewPathDetails("Shared", "Error"))) ;
        }


        public async Task<IActionResult> GetLeadsByDate()
        {

            var CustomerDetailList = await _ICustomerDetailRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            var CustomerLeadLIst = await _ICustomerLeadRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            var responseDetails = (from CDList in CustomerDetailList.Entities
                                   join CLList in CustomerLeadLIst.Entities
                                   on CDList.Id equals CLList.CustomerId
                                   where CLList.EmpId == Convert.ToInt32(HttpContext.Session.GetString("empId"))
                                   select new CustomerDetail
                                   {
                                       LeadName = CDList.LeadName,
                                       Location = CDList.Location,
                                       Phone = CDList.Phone,
                                       Email = CDList.Email,
                                       Description_Project = CDList.Description_Project,
                                       AssignDate = CDList.AssignDate
                                       ,
                                       SpecialRemarks = CDList.SpecialRemarks

                                   }).ToList();

            var Leads = new List<LeadsDetail>();
            foreach (var item in responseDetails.GroupBy(x => x.AssignDate))
            {
                Leads.Add(new LeadsDetail()
                {
                    NoOfLeads = item.Count() + "  Leads",
                    AssignDate = item.First().AssignDate,
                });
            }
            return Json(Leads);
        }
    }
}
