using HRMS.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Core.Entities.LeadManagement
{
    [Table("CustomerLeadDetail", Schema = "LeadManagement")]
    public class CustomerLeadDetail: BaseModel<int>
    {
        public int EmpId { get; set; }
        public int CustomerId { get; set; }
        public int LeadType { get; set; }
        public string Description { get; set; }
    }
}
