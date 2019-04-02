using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ET.Models.ViewModel
{
    public class Login
    {
        [Required(ErrorMessage = "Boş bırakılmaz")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Boş bırakılmaz")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name ="Remember Me?")]
        public bool rememberMe { get; set; }
    }
}