using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBDotCom.Areas.Admin.Models
{
    public class CreateUserViewModel : UserViewModel
    {
        public string Password { get; set; }
    }
}
