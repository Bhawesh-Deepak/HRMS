﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers.LeadCloserFrom
{
    public class LeadCloser : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
