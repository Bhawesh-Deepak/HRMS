using HRMS.Core.Entities.LeadManagement;
using HRMS.Core.Helpers.CommonHelper;
using HRMS.Services.Repository.GenericRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers.LeadCloserFrom
{
    public class LeadCloser : Controller
    {
        private readonly IGenericRepository<CustomerLead, int> _ICustomerLeadRepository;
        private readonly IGenericRepository<CustomerDetail, int> _ICustomerDetailRepository;
        public LeadCloser(IGenericRepository<CustomerLead, int> customerLeadRepository, IGenericRepository<CustomerDetail, int> iCustomerDetailRepository)
        {
            _ICustomerDetailRepository = iCustomerDetailRepository;
            _ICustomerLeadRepository = customerLeadRepository;
        }
        public async Task<IActionResult> Index(int customerId)
        {
            var CustomerLead = await _ICustomerLeadRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            var CustomerDetail = await _ICustomerDetailRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            var CustomerDetails = (from CLead in CustomerLead.Entities
                                   join CDList in CustomerDetail.Entities
                                   on CLead.CustomerId equals CDList.Id
                                   select new CustomerDetail
                                   {
                                       Id = CDList.Id,
                                       LeadName = CDList.LeadName,
                                       Location = CDList.Location,
                                       Email = CDList.Email,
                                       Phone = CDList.Phone,
                                       EmpCode = CLead.LeadCode
                                       ,
                                       SpecialRemarks = CDList.SpecialRemarks,
                                       Description_Project = CDList.Description_Project

                                   }).ToList();


            return View(CustomerDetails);
        }

        public async Task<IActionResult> GetCustomerLCF(int customerId)
        {
            return await Task.Run(() => PartialView(ViewHelper.GetViewPathDetails("LeadCloser","_CustomerLCFPartial")));
        }
    }
}
