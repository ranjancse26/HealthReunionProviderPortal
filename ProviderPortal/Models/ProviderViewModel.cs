using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProviderPortal.Models
{
    public class ProviderViewModel
    {
        [UIHint("Hidden")]
        public int ProviderId { get; set; }
     
        [Required]
        [Display(Name = "Provider Name")]
        public string ProviderName { get; set; }
        
        [Required]
        [Display(Name = "Provider Description")]
        [DataType(DataType.MultilineText)]
        public string ProviderDescription { get; set; }
        
        [Required]
        [Display(Name = "Privacy Statement")]
        [DataType(DataType.MultilineText)]
        public string PrivacyStatement { get; set; }
       
        [Required]
        [Display(Name = "Authorization Reason")]
        [DataType(DataType.MultilineText)]
        public string AuthorizationReason { get; set; }
        
        [Required]
        [Display(Name = "Terms Of Use")]
        [DataType(DataType.MultilineText)]       
        public string TermsOfUse { get; set; }
       
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }     
    }
}