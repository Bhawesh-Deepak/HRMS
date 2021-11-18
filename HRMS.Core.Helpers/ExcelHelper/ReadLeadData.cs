using HRMS.Core.Entities.LeadManagement;
using HRMS.Core.Entities.Payroll;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace HRMS.Core.Helpers.ExcelHelper
{
    public class ReadLeadData
    {
        public IEnumerable<CustomerDetail> GetCustomerDetail(IFormFile inputFile)
        {
            var dataResult = ReadExcelDataHelper.GetDataTableFromExcelFile(inputFile);
            var models = new List<CustomerDetail>();

            for (int i = 1; i < dataResult.dtResult.Rows.Count; i++)
            {
                var model = new CustomerDetail();
                model.CustomerName =dataResult.dtResult.Rows[i][0].ToString();
                model.Address = dataResult.dtResult.Rows[i][1].ToString();
                model.Phone = dataResult.dtResult.Rows[i][2].ToString();
                model.Email = dataResult.dtResult.Rows[i][3].ToString();
                model.Description = dataResult.dtResult.Rows[i][4].ToString();
                model.Country = string.Empty;
                model.State = string.Empty;
                model.City = string.Empty;
                model.ZipCode = string.Empty;
                model.CompanyName = string.Empty;
                model.Website = string.Empty;
                model.Industry = string.Empty;
                
                model.FinancialYear = 1;
                models.Add(model);
            }

            return models;
        }
    }
}
