using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HBDotCom.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HBDotCom.Areas.Identity.Controllers
{
    public class IdentityController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public bool UserNameCheck(string username)
        {
            username = username.ToLower();
            ApplicationUser user = _userManager.FindByNameAsync(username).Result;

            if (user != null)
                return false;

            return true;

            
        }
    }
}