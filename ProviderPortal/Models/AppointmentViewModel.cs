using PatientPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProviderPortal.Models
{
    public class AppointmentViewModelEntity
    {
        public int AppointmentId { get; set; }
        public int ProviderId { get; set; }
        public int PatientId { get; set; }
        public System.DateTime AppointmentDate { get; set; }
        public string ReasonForVisit { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }
    }

    public class AppointmentViewModel
    {
        public AppointmentViewModel()
        {
            this.AppointmentViewModelEntity = new AppointmentViewModelEntity();
            this.Patients = new DrodownItemsViewModel();
        }
        public AppointmentViewModelEntity AppointmentViewModelEntity { get; set; }
        public DrodownItemsViewModel Patients { get; set; }
    }
}