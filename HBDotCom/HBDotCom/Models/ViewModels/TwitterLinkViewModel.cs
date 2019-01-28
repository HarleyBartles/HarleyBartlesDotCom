using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HBDotCom.Models.ViewModels
{
    public class TwitterLinkViewModel
    {
        [Display(Name ="Twitter Username")]
        public string TwitterName { get; set; }
    }
}
