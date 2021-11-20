using ClosedXML.Excel;
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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [CustomAuthenticate]
    public class HomeController : Controller
    {
        private readonly IGenericRepository<CustomerDetail, int> _ICustomerDetailRepository;
        private readonly IGenericRepository<LeadType, int> _ILeadTypeRepository;
        private readonly IGenericRepository<CustomerLeadDetail, int> _ICustomerLeadRepository;
        private readonly ILogger<HomeController> _ILogger;
        public HomeController(IGenericRepository<CustomerDetail, int> iCustomerDetailRepository, IGenericRepository<LeadType, int> iLeadTypeRepository,
              IGenericRepository<CustomerLeadDetail, int> customerLeadRepo, ILogger<HomeController> logger)
        {
            _ICustomerDetailRepository = iCustomerDetailRepository;
            _ILeadTypeRepository = iLeadTypeRepository;
            _ICustomerLeadRepository = customerLeadRepo;
            _ILogger = logger;
        }
        public IActionResult Index()
        {
            Serilog.Log.Information("Home Index action method called !!!");
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> Error()
        {
            return await Task.Run(() => View(ViewHelper.GetViewPathDetails("Shared", "Error")));
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
        public async Task<IActionResult> ExportCustomerLead(int? LeadTypeId)
        {
            List<CompleteLeadsDetailVM> responseDetails = await GetCustomerDetailLeadTypeWise(LeadTypeId);

            DataTable dt = new DataTable("LeadDetails");
            dt.Columns.AddRange(new DataColumn[8] {
                    new DataColumn("LeadName"),
                    new DataColumn("Location"),
                    new DataColumn("Phone"),
                    new DataColumn("Email"),
                    new DataColumn("Description/Project"),
                    new DataColumn("Special Remarks"),
                    new DataColumn("AssignDate"),
                     new DataColumn("Status"),
            });

            foreach (var data in responseDetails)
            {
                dt.Rows.Add(data.LeadName, data.Location, data.Phone, data.Email, data.Description_Project, data.SpecialRemarks, data.AssignDate.ToString("dd/MM/yyyy"), data.LeadTypeName);
            }

            using XLWorkbook wb = new XLWorkbook();
            wb.Worksheets.Add(dt);
            using MemoryStream stream = new MemoryStream();
            wb.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "CustomerLeadDetails.xlsx");
        }
        private async Task<List<CompleteLeadsDetailVM>> GetCustomerDetailLeadTypeWise(int? LeadTypeId)
        {
            var CustomerDetailList = await _ICustomerDetailRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            var CustomerLeadLIst = await _ICustomerLeadRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            var LeadTypeLIst = await _ILeadTypeRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            if (LeadTypeId == null)
            {
                var responseDetails = (from CDList in CustomerDetailList.Entities
                                       join CLList in CustomerLeadLIst.Entities
                                       on CDList.Id equals CLList.CustomerId
                                       join ltype in LeadTypeLIst.Entities on CLList.LeadType equals ltype.Id into leadtype
                                       from subpet in leadtype.DefaultIfEmpty()
                                       where CLList.EmpId == Convert.ToInt32(HttpContext.Session.GetString("empId"))
                                       select new CompleteLeadsDetailVM
                                       {
                                           CustomerId = CDList.Id,
                                           LeadName = CDList.LeadName,
                                           Location = CDList.Location,
                                           Phone = CDList.Phone,
                                           Email = CDList.Email,
                                           Description_Project = CDList.Description_Project,
                                           AssignDate = CDList.AssignDate,
                                           SpecialRemarks = CDList.SpecialRemarks,
                                           LeadTypeName = subpet?.Name ?? String.Empty
                                       }).ToList();
                return responseDetails;
            }
            else
            {
                var responseDetails = (from CDList in CustomerDetailList.Entities
                                       join CLList in CustomerLeadLIst.Entities
                                       on CDList.Id equals CLList.CustomerId
                                       join ltype in LeadTypeLIst.Entities on CLList.LeadType equals ltype.Id into leadtype
                                       from subpet in leadtype.DefaultIfEmpty()
                                       where CLList.EmpId == Convert.ToInt32(HttpContext.Session.GetString("empId")) && CLList.LeadType == LeadTypeId
                                       select new CompleteLeadsDetailVM
                                       {
                                           CustomerId = CDList.Id,
                                           LeadName = CDList.LeadName,
                                           Location = CDList.Location,
                                           Phone = CDList.Phone,
                                           Email = CDList.Email,
                                           Description_Project = CDList.Description_Project,
                                           AssignDate = CDList.AssignDate,
                                           SpecialRemarks = CDList.SpecialRemarks,
                                           LeadTypeName = subpet?.Name ?? String.Empty
                                       }).ToList();
                return responseDetails;
            }

        }
    }
}
