using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HBDotCom.Areas.Identity.Controllers
{
    public class IdentityController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public bool UserNameCheck(string username)
        {
            username = username.ToLower();
            IdentityUser user = _userManager.FindByNameAsync(username).Result;

            if (user != null)
                return false;

            return true;

            
        }
    }
}