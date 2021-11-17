using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Core.ReqRespVm.Response.Leads
{
   public class LeadsBySupervisorVM
    {
        public string CustomerName { get; set; }
        public string Level { get; set; }
        public int Leads { get; set; }
        public int Called { get; set; }
        public int Pending { get; set; }
    }
}
