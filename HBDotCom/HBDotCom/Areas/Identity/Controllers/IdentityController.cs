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

        public async Task<bool> NameAvailabilityCheck(string name)
        {
            name = name.ToLower();
            ApplicationUser user = await _userManager.FindByNameAsync(name);

            if (user != null)
                return false;

            user = await _userManager.FindByEmailAsync(name);
            if (user != null)
                return false;

            return true;

            
        }
    }
}