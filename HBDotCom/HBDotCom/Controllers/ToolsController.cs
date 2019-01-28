using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HBDotCom.Controllers
{
    public class ToolsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public string MakeSearchLink(string userName)
        {
            userName = userName.Replace("@", "");
            return "https://twitter.com/search?from:" + userName + "%20-filter:replies";
        }
    }
}