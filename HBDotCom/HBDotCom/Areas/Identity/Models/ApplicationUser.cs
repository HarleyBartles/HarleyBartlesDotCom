using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HBDotCom.Areas.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        [NotMapped]
        public string FirstName { get { return FullName.Substring(0, FullName.IndexOf(" ")); } }
        [NotMapped]
        public string LastName { get { return FullName.Substring(FullName.LastIndexOf(" ")); } }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }

    }
}
