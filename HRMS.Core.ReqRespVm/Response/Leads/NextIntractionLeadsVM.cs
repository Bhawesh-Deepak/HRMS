﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Core.ReqRespVm.Response.Leads
{
   public class NextIntractionLeadsVM
    {
        public string CustomerName { get; set; }
        public DateTime ? NextIntractionDate { get; set; }
        public string NextIntractionActivity { get; set; }
        public TimeSpan ? NextIntractionTime { get; set; }
    }
}
