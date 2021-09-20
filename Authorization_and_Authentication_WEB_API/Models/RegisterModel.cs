using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization_and_Authentication_WEB_API.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email is required")]
      
        public string UserName { get; set; }


        [Required(ErrorMessage ="Email is required")]
        [EmailAddress]
        public string Email { get; set; }


        [Required(ErrorMessage ="Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        public string Password { get; set; }

       
    }
}
