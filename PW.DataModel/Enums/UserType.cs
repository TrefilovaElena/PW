using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PW.DataModel.Enums
{
    public enum UserType
    {
        [Display(Name = "System")]
        System = 1,

        [Display(Name = "Client")]
        Client = 2,
 
    }
}
