using PatientPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProviderPortal.Models
{
    public class UserViewEnity
    {
        [UIHint("Hidden")]
        public int UserId { get; set; }

        [UIHint("Hidden")]
        public int ProviderId { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserViewModel
    {
        public UserViewModel()
        {
            this.UserViewEntity = new UserViewEnity();
            this.UserViewEntityGrid = new List<UserViewEnity>();
        }
        public UserViewEnity UserViewEntity { get; set; }
        public List<UserViewEnity> UserViewEntityGrid { get; set; }
    }
}