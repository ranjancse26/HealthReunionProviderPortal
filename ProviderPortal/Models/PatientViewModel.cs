using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProviderPortal.Models
{
    public class PatientViewModel
    {
        public PatientViewModel()
        {
            this.PatientViewEntity = new PatientViewEntity();
            this.PatientViewModelGrid = new List<PatientViewEntity>();
        }

        public List<PatientViewEntity> PatientViewModelGrid { get; set; }
        public PatientViewEntity PatientViewEntity { get; set; }
    }

    public class PatientViewEntity
    {
        [UIHint("Hidden")]
        public int PatientId { get; set; }
        
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }
     
        [Required]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "MiddleName")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public string DOB { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [UIHint("Hidden")]
        public bool Sex { get; set; }

        [Required]
        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]       
        public string Address { get; set; }

        [Required]
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]       
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }
    }
}