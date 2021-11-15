using HRMS.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Core.Entities.LeadManagement
{
    [Table("CustomerDetail", Schema = "LeadManagement")]
    public class CustomerDetail:BaseModel<int>
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
    }
}
