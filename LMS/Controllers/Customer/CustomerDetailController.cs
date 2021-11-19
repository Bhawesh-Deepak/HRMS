﻿using ClosedXML.Excel;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.LeadManagement;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Helpers.CommonHelper;
using HRMS.Core.Helpers.ExcelHelper;
using HRMS.Services.Repository.GenericRepository;
using LMS.Controllers.CustomAction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers.Customer
{
    public class CustomerDetailController : Controller
    {
        private readonly IGenericRepository<CustomerDetail, int> _ICustomerDetailRepository;
        private readonly IGenericRepository<EmployeeDetail, int> _IEmployeeDetailRepository;
        private readonly IGenericRepository<CustomerLeadDetail, int> _ICustomerLeadRepository;
        private readonly IGenericRepository<CustomerSecondryDetail, int> _ICustomerSecondryDetailRepository;
        private readonly IGenericRepository<LeadType, int> _ILeadTypeRepository;

        public CustomerDetailController(IGenericRepository<CustomerDetail, int> iCustomerDetailRepository,
            IGenericRepository<EmployeeDetail, int> employeeDetailRepo,
            IGenericRepository<CustomerLeadDetail, int> customerLeadRepo,
            IGenericRepository<CustomerSecondryDetail, int> customerSecondryDetailRepo,
            IGenericRepository<LeadType, int> leadTypeRepository)
        {
            _ICustomerDetailRepository = iCustomerDetailRepository;
            _IEmployeeDetailRepository = employeeDetailRepo;
            _ICustomerLeadRepository = customerLeadRepo;
            _ICustomerSecondryDetailRepository = customerSecondryDetailRepo;
            _ILeadTypeRepository = leadTypeRepository;
        }

        public IActionResult Index()
        {
            return View("~/Views/Customer/LeadManagement.cshtml");
        }

        public async Task<IActionResult> CustomerList(DateTime AssignDate)
        {
            List<CustomerDetail> responseDetails = await GetCustomerDetaiAsignDateWise(AssignDate);

            return await Task.Run(() => View(ViewHelper.GetViewPathDetails("Customer", "GetCustomerList"), responseDetails));
        }

        public async Task<IActionResult> ExportCustomerDetail(DateTime AssignDate)
        {
            List<CustomerDetail> responseDetails = await GetCustomerDetaiAsignDateWise(AssignDate);

            DataTable dt = new DataTable("LeadDetails");
            dt.Columns.AddRange(new DataColumn[6] {
                    new DataColumn("LeadName"),
                    new DataColumn("Location"),
                    new DataColumn("Phone"),
                    new DataColumn("Email"),
                    new DataColumn("Description_Project"),
                    new DataColumn("AssignDate"),
            });

            foreach (var data in responseDetails)
            {
                dt.Rows.Add(data.LeadName, data.Location, data.Phone, data.Email, data.Description_Project, data.AssignDate.ToString());
            }

            using XLWorkbook wb = new XLWorkbook();
            wb.Worksheets.Add(dt);
            using MemoryStream stream = new MemoryStream();
            wb.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "CustomerLeadDetails.xlsx");
        }



        [HttpPost]
        public async Task<IActionResult> UploadLeadData(DateTime AssignDate, IFormFile CustomerData)
        {
            try
            {
                var data = new ReadLeadData().GetCustomerDetail(CustomerData);
                data.ToList().ForEach(x =>
                {
                    x.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("empId"));
                    x.AssignDate = AssignDate;
                });

                var response = await _ICustomerDetailRepository.CreateEntities(data.ToArray());

                if (response.ResponseStatus == ResponseStatus.Success)
                {
                    await LeadDistribution(data.ToList());

                    return Json("Customer uploaded !!!");
                }

                return RedirectToAction("Error", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadLeadWithEmpCode(DateTime AssignDate, IFormFile CustomerData)
        {
            var data = new ReadLeadData().GetCustomerDetailWithEmpCode(CustomerData);

            data.ToList().ForEach(x =>
            {
                x.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("empId"));
                x.AssignDate = AssignDate;
            });

            var response = await _ICustomerDetailRepository.CreateEntities(data.ToArray());

            if (response.ResponseStatus == ResponseStatus.Success)
            {

                var empDetails = await _IEmployeeDetailRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

                var customerDetails = await _ICustomerDetailRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

                var dbModels = new List<CustomerLeadDetail>();

                data.ToList().ForEach(item =>
                {
                    var customer = customerDetails.Entities.FirstOrDefault(x => x.LeadName == item.LeadName
                     && x.Email == item.Email && x.Phone == item.Phone && x.Location == item.Location);

                    var empData = empDetails.Entities.FirstOrDefault(x => x.EmpCode.ToLowerInvariant().Trim()
                                         == item.EmpCode.ToLowerInvariant().Trim());

                    var custLeadModel = new CustomerLeadDetail()
                    {
                        Activity = string.Empty,
                        Comment = string.Empty,
                        EmpId = empData.Id,
                        CustomerId = customer.Id,
                        LeadType = 0,
                        Description = string.Empty,
                        NextIntractionActivity= string.Empty
                    };

                    dbModels.Add(custLeadModel);

                });

                var dbResponse = await _ICustomerLeadRepository.CreateEntities(dbModels.ToArray());

                return Json("Customer uploaded !!!");
            }

            return RedirectToAction("Error", "Home");


        }
        [HttpGet]
        public async Task<IActionResult> GetCustomerDetail(int custId)
        {
            ViewBag.CustomerId = custId;
            var response = await _ICustomerDetailRepository.GetAllEntityById(x => x.Id == custId);
            return View(ViewHelper.GetViewPathDetails("Customer", "CustomerDetail"), response.Entity);
        }
        [HttpGet]
        public async Task<IActionResult> AddSecondryDetail(int customerId, string phone, string emailId)
        {
            var phoneDetail = new CustomerSecondryDetail()
            {
                CustomerId = customerId,
                Phone = phone,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = 1,
                Email = emailId,
                CreatedDate = DateTime.Now
            };
            var response = await _ICustomerSecondryDetailRepository.CreateEntity(phoneDetail);
            return Json("Phone added successfully");
        }
        public async Task<IActionResult> GetSecondryDetails(int custId)
        {
            var response = await _ICustomerSecondryDetailRepository.GetAllEntities(x => x.CustomerId == custId && x.IsActive && !x.IsDeleted);
            return PartialView(ViewHelper.GetViewPathDetails("Customer", "CustomerSecondryDetails"), response.Entities);
        }

        public async Task<IActionResult> UpdateSecondryDetail(int id, string phone, string emailId)
        {
            var updateModel = await _ICustomerSecondryDetailRepository.GetAllEntityById(x => x.Id == id);
            updateModel.Entity.Phone = phone;
            updateModel.Entity.Email = emailId;

            var response = await _ICustomerSecondryDetailRepository.UpdateEntity(updateModel.Entity);
            return Json("Phone added successfully");
        }

        public async Task<IActionResult> GetSecondryDetail(int id)
        {
            var response = await _ICustomerSecondryDetailRepository.GetAllEntityById(x => x.Id == id);
            return Json(response.Entity);
        }
        public async Task<IActionResult> DeleteSecondryDetails(int id)
        {
            var deleteModel = await _ICustomerSecondryDetailRepository.GetAllEntityById(x => x.Id == id);
            deleteModel.Entity.IsActive = false;
            deleteModel.Entity.IsDeleted = true;

            var response = await _ICustomerSecondryDetailRepository.DeleteEntity(deleteModel.Entity);

            return Json("Detail deleted successfully !!!");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBasicInfo(CustomerDetail model)
        {
            model.UpdatedDate = DateTime.Now;
            model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("empId"));
            model.CreatedDate = DateTime.Now;
            var response = await _ICustomerDetailRepository.UpdateEntity(model);
            return Json(response.Message);
        }

        public async Task<IActionResult> GetLeadTypeJson()
        {
            var response = await _ILeadTypeRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            return Json(response.Entities);
        }

        public async Task<IActionResult> GetCustomerLeadDetail(int custId)
        {
            var response = await _ICustomerLeadRepository.GetAllEntityById(x => x.CustomerId == custId);
            return PartialView(ViewHelper.GetViewPathDetails("Customer", "CustomerIntractionPartial"), response.Entity);
        }

        [HttpPost]
        public async Task<IActionResult> PostCustomerLeadDetail(CustomerLeadDetail model)
        {
            var updateResponse = await _ICustomerLeadRepository.GetAllEntityById(x => x.Id == model.Id);
            updateResponse.Entity.IntractionDate = model.IntractionDate;
            updateResponse.Entity.IntractionTime = model.IntractionTime;
            updateResponse.Entity.NextIntractionActivity = model.NextIntractionActivity;
            updateResponse.Entity.Activity = model.Activity;
            updateResponse.Entity.NextIntractionDate = model.NextIntractionDate;
            updateResponse.Entity.NextIntractionTime = model.NextIntractionTime;
            updateResponse.Entity.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("empId"));
            updateResponse.Entity.UpdatedDate = DateTime.Now;
            updateResponse.Entity.Comment = model.Comment;
            updateResponse.Entity.LeadType = model.LeadType;
            var response = await _ICustomerLeadRepository.UpdateEntity(updateResponse.Entity);

            return Json("Customer lead updated successfully !!!");
        }



        private async Task LeadDistribution(List<CustomerDetail> model)
        {
            var empCode = HttpContext.Session.GetString("empCode");

            var superVisorEmployees = await _IEmployeeDetailRepository.GetAllEntities(x => x.SuperVisorCode.Trim().ToUpper() == empCode.Trim().ToUpper());

            int perEmpCout = Convert.ToInt32(model?.Count / superVisorEmployees?.Entities.Count());

            int remainingDeals = Convert.ToInt32(model?.Count % superVisorEmployees?.Entities.Count());

            IDictionary<int, List<CustomerDetail>> empCustomerMapping = new Dictionary<int, List<CustomerDetail>>();
            IDictionary<int, List<CustomerDetail>> remainingempCustomerMapping = new Dictionary<int, List<CustomerDetail>>();

            for (int i = 0; i < superVisorEmployees.Entities.Count(); i++)
            {
                var takenCount = perEmpCout * i;
                empCustomerMapping.Add(superVisorEmployees.Entities.ElementAt(i).Id, model.Skip(takenCount).Take(perEmpCout).ToList());
            }

            if (remainingDeals > 0)
            {
                var empId = (superVisorEmployees.Entities.ElementAt(superVisorEmployees.Entities.Count() - 1).Id);
                int skipCount = model.Count - remainingDeals;

                remainingempCustomerMapping.Add(empId,
                    model.Skip(skipCount).Take(remainingDeals).ToList());
            }

            var dbModels = new List<CustomerLeadDetail>();

            empCustomerMapping.ToList().ForEach(x =>
            {
                x.Value.ForEach(item =>
                {
                    var dbModel = new CustomerLeadDetail();

                    dbModel.EmpId = x.Key;
                    dbModel.CustomerId = item.Id;
                    dbModel.LeadType = 0;
                    dbModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("empId"));
                    dbModel.CreatedDate = DateTime.Now;
                    dbModels.Add(dbModel);
                });
            });

            remainingempCustomerMapping.ToList().ForEach(x =>
            {
                x.Value.ForEach(item =>
                {
                    var dbModel = new CustomerLeadDetail();

                    dbModel.EmpId = x.Key;
                    dbModel.CustomerId = item.Id;
                    dbModel.LeadType = 0;
                    dbModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("empId"));
                    dbModel.CreatedDate = DateTime.Now;
                    dbModels.Add(dbModel);
                });
            });


            var response = await _ICustomerLeadRepository.CreateEntities(dbModels.ToArray());
        }

        private async Task<List<CustomerDetail>> GetCustomerDetaiAsignDateWise(DateTime AssignDate)
        {
            var CustomerDetailList = await _ICustomerDetailRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

            var CustomerLeadLIst = await _ICustomerLeadRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

            var responseDetails = (from CDList in CustomerDetailList.Entities
                                   join CLList in CustomerLeadLIst.Entities
                                   on CDList.Id equals CLList.CustomerId
                                   where CLList.EmpId == Convert.ToInt32(HttpContext.Session.GetString("empId")) && CDList.AssignDate == AssignDate
                                   select new CustomerDetail
                                   {
                                       LeadName = CDList.LeadName,
                                       Location = CDList.Location,
                                       Phone = CDList.Phone,
                                       Email = CDList.Email,
                                       Description_Project = CDList.Description_Project,
                                       AssignDate = CDList.AssignDate,
                                       SpecialRemarks = CDList.SpecialRemarks


                                   }).ToList();
            return responseDetails;
        }
    }
}
