﻿using HRMS.Core.Entities.LeadManagement;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Helpers.ExcelHelper;
using HRMS.Services.Repository.GenericRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers.Customer
{
    public class CustomerDetailController : Controller
    {
        private readonly IGenericRepository<CustomerDetail, int> _ICustomerDetailRepository;
        private readonly IGenericRepository<EmployeeDetail, int> _IEmployeeDetailRepository;
        private readonly IGenericRepository<CustomerLeadDetail, int> _ICustomerLeadRepository;

        public CustomerDetailController(IGenericRepository<CustomerDetail, int> iCustomerDetailRepository,
            IGenericRepository<EmployeeDetail, int> employeeDetailRepo, IGenericRepository<CustomerLeadDetail, int> customerLeadRepo)
        {
            _ICustomerDetailRepository = iCustomerDetailRepository;
            _IEmployeeDetailRepository = employeeDetailRepo;
            _ICustomerLeadRepository = customerLeadRepo;
        }

        public IActionResult Index()
        {
            return View("~/Views/Customer/LeadManagement.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> UploadLeadData(IFormFile CustomerData)
        {
            var data = new ReadLeadData().GetCustomerDetail(CustomerData);

            data.ToList().ForEach(x =>
            {
                x.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("empId"));

            });

            var response = await _ICustomerDetailRepository.CreateEntities(data.ToArray());

            await LeadDistribution(data.ToList());

            return Json("Customer uploaded !!!");

        }

        private async Task LeadDistribution(List<CustomerDetail> model)
        {
            var empCode = HttpContext.Session.GetString("empCode");

            var superVisorEmployees = await _IEmployeeDetailRepository.GetAllEntities(x => x.SuperVisorCode.Trim().ToUpper() == empCode.Trim().ToUpper());

            int perEmpCout = Convert.ToInt32(model?.Count / superVisorEmployees?.Entities.Count());

            IDictionary<int, List<CustomerDetail>> empCustomerMapping = new Dictionary<int, List<CustomerDetail>>();

            for (int i = 0; i < superVisorEmployees.Entities.Count(); i++)
            {
                var takenCount = perEmpCout * i;
                empCustomerMapping.Add(superVisorEmployees.Entities.ElementAt(i).Id, model.Skip(takenCount).Take(perEmpCout).ToList());
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

            var response = await _ICustomerLeadRepository.CreateEntities(dbModels.ToArray());
        }
    }
}