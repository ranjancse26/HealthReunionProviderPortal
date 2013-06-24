using PatientPortal.Models;
using ProviderPortal.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ProviderPortal.Models
{
    public class PatientDocumentViewEntity
    {
        public DrodownItemsViewModel Patients { get; set; }

        [Required]
        [Display(Name = "Document Type")]
        public string DocumentType { get; set; }

        [Display(Name = "Document To Upload")]
        [ValidateFile(ErrorMessage = "Please select *.xml file")]       
        public HttpPostedFileBase DocumentToUpload { get; set; }
    }

    public class PatientDocumentViewModel
    {
        public PatientDocumentViewModel()
        {
            this.PatientDocumentViewEntity = new PatientDocumentViewEntity();
            this.PatientDocumentsGrid = new List<PatientDocumentsModel>();
        }

        public PatientDocumentViewEntity PatientDocumentViewEntity { get; set; }
        public List<PatientDocumentsModel> PatientDocumentsGrid { get; set; }
    }
}