using HealthReunionDataAccess;
using PatientPortal.Models;
using ProviderPortal.Helpers;
using ProviderPortal.Models;
using ProviderPortal.RenderXSLTExample;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ProviderPortal.Controllers
{
    public class PatientController : Controller
    {
        [HttpGet]
        public ActionResult PatientRegistration()
        {
            if (!LogOnHelper.CheckLogOn()) return RedirectToAction("Login", "Account");

            var patientViewModel = new PatientViewModel();
            patientViewModel.PatientViewModelGrid = GetPatientsForLoggedInProvider();            
            return View(patientViewModel);
        }

        private List<PatientViewEntity> GetPatientsForLoggedInProvider()
        {
            var patientViewEntityList = new List<PatientViewEntity>();
            PatientRepository patientRepository = new PatientRepository();
            var patientsList = patientRepository.GetAllPatients(int.Parse(Session["ProviderId"].ToString()));
            foreach (var patient in patientsList)
            {
                patientViewEntityList.Add(new PatientViewEntity
                {
                    PatientId = patient.PatientId,
                    FirstName = patient.FirstName,
                    MiddleName = patient.MiddleName,
                    LastName = patient.LastName,
                    City = patient.City,
                    State = patient.State,
                    Country = patient.Country,
                    DOB = patient.DOB.ToShortDateString(),
                    Phone = patient.Phone
                });
            }
            return patientViewEntityList;
        }

        [HttpGet]
        public JsonResult GetPatientById(int patientId)
        {
            var patientRepository = new PatientRepository();
            var patient = patientRepository.GetPatientById(patientId);
            var userName = patientRepository.GetUserNameByPatientId(patientId);
            var patientViewEntity = new PatientViewEntity
            {
                PatientId = patient.PatientId,
                FirstName = patient.FirstName,
                MiddleName = patient.MiddleName,
                LastName = patient.LastName,
                City = patient.City,
                State = patient.State,
                Country = patient.Country,
                DOB = patient.DOB.Date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.GetCultureInfo("en-US")),
                Phone = patient.Phone,
                Address = patient.Address,
                Email = patient.Email,
                Sex = patient.Sex,
                UserName = userName 
            };
            return Json(patientViewEntity, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult GetPatientDocuments(int patientId)
        {
            DocumentRepository documentRepository = new DocumentRepository();
            int providerId = int.Parse(Session["ProviderId"].ToString());

            var patientDocuments = documentRepository.GetDocuments(patientId, providerId);

            var ccrXslPath = Server.MapPath("~/ccr.xsl");
            var ccdXslPath = Server.MapPath("~/ccd.xsl");
            foreach (var doc in patientDocuments)
            {
                if (doc.DocumentText.Contains("ContinuityOfCareRecord"))
                    doc.DocumentText = HtmlHelperExtensions.RenderXslt(ccrXslPath, doc.DocumentText).ToHtmlString();
                else if (doc.DocumentText.Contains("ClinicalDocument"))
                    doc.DocumentText = HtmlHelperExtensions.RenderXslt(ccdXslPath, doc.DocumentText).ToHtmlString();
            }

            return PartialView("PatientDocumentsGrid", patientDocuments);
        }
        
        [HttpGet]
        public ActionResult AddDocuments(int? patientId)
        {
            if (!LogOnHelper.CheckLogOn()) return RedirectToAction("Login", "Account");

            var patientDocumentViewModel = new PatientDocumentViewModel();
            patientDocumentViewModel.PatientDocumentViewEntity.Patients = GetPatients(patientId);
            if (patientId != null)
            {
                patientDocumentViewModel.PatientDocumentViewEntity.Patients.SelectedItemId = patientId.ToString();
            }
            return View(patientDocumentViewModel);
        }

        /// <summary>
        /// Http Post for saving the Patient Documents
        /// </summary>
        /// <param name="patientDocumentViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddDocuments(PatientDocumentViewModel patientDocumentViewModel)
        {
            if (!ModelState.IsValid)
            {
                patientDocumentViewModel.PatientDocumentViewEntity.Patients = GetPatients();
                return View(patientDocumentViewModel);
            }

            if (patientDocumentViewModel.PatientDocumentViewEntity.Patients.SelectedItemId == "-1")
            {
                ModelState.AddModelError("", "Please select patient");
                patientDocumentViewModel.PatientDocumentViewEntity.Patients = GetPatients();
                return View(patientDocumentViewModel);
            }

            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            patientDocumentViewModel.PatientDocumentViewEntity.DocumentToUpload.InputStream.CopyTo(memoryStream);
            byte[] documentInByteArray = memoryStream.ToArray();

            Document document = new Document();
            DocumentRepository documentRepository = new DocumentRepository();

            document.ProviderId = int.Parse(Session["ProviderId"].ToString());
            document.PatientId = int.Parse(patientDocumentViewModel.PatientDocumentViewEntity.Patients.SelectedItemId);
            document.DocumentType = patientDocumentViewModel.PatientDocumentViewEntity.DocumentType;
            document.Document1 = documentInByteArray;
            document.CreationTime = DateTime.Now;
            documentRepository.AddDocuments(document);

            return RedirectToAction("AddDocuments", new { patientId = document.PatientId });
        }

        private DrodownItemsViewModel GetPatients(int? patientId = -1)
        {
            var dropDownList = new DrodownItemsViewModel();
            var patientsList = new List<SelectListItem>();
            var patientRepository = new PatientRepository();
            SelectListItem selectedListItem = null;

            var patients = patientRepository.GetAllPatients(int.Parse(Session["ProviderId"].ToString()));
            foreach (var patient in patients)
            {
                selectedListItem = new SelectListItem
                {
                    Text = patient.FirstName + " " + patient.MiddleName + " " + patient.LastName,
                    Value = patient.PatientId.ToString(),
                    Selected = false 
                };

                if (patientId != null && patient.PatientId == patientId)
                {
                    selectedListItem.Selected = true;
                    patientsList.Add(selectedListItem);
                }
                else
                {                    
                    patientsList.Add(selectedListItem);
                }
            }

            var defaultItem = new SelectListItem
            {
                Value = "-1",
                Text = "Select",
                Selected = false 
            };

            if (patientId == null || patientId == -1)
            {
                defaultItem.Selected = true;
                patientsList.Add(defaultItem);
            }
            else
            {
                patientsList.Add(defaultItem);
            }
            dropDownList.Items = patientsList;           
            return dropDownList;
        }

        [HttpGet]
        public ActionResult PatientAppointments()
        {
            if (!LogOnHelper.CheckLogOn()) return RedirectToAction("Login", "Account");

            var appointmentViewModel = new AppointmentViewModel();
            appointmentViewModel.Patients = GetPatients();
            return View(appointmentViewModel);
        }

        private List<Appointment> GetAppointmentsForPatient(int patientId)
        {
            var appointmentRepository = new AppointementRepository();
            return appointmentRepository.GetAppointmentsByPatientID(patientId, DateTime.Now.Date.ToString());           
        }
        
        [HttpPost]
        public ActionResult GetPatientAppointments(int patientId, string appointmentDate)
        {
            var appointmentRepository = new AppointementRepository();
            var appointmentViewModelList = new List<AppointmentViewModelEntity>();
            var appointments = appointmentRepository.GetAppointmentsByPatientID(patientId, appointmentDate);
            foreach (var appointment in appointments)
            {
                appointmentViewModelList.Add(new AppointmentViewModelEntity
                {
                    PatientId = appointment.PatientId,
                    AppointmentId = appointment.AppointmentId,
                    AppointmentDate = appointment.AppointmentDate,
                    ProviderId = appointment.ProviderId,
                    ReasonForVisit = appointment.ReasonForVisit,
                    Status = appointment.Status,
                    Time = appointment.Time
                });
            }
            return PartialView("PatientAppointmentGrid", appointmentViewModelList);
        }

        [HttpPost]
        public ActionResult PatientRegistration(PatientViewModel patientViewModel)
        {
            if (!LogOnHelper.CheckLogOn()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(patientViewModel);
            }
            try
            {
                HealthReunionDataAccess.HealthReunionEntities dataContext = new HealthReunionDataAccess.HealthReunionEntities();
                PatientRepository patientRepository = new PatientRepository();
                
                Patient patient = new Patient();
                patient.PatientId = patientViewModel.PatientViewEntity.PatientId;
                patient.ProviderId = int.Parse(Session["ProviderId"].ToString());
                patient.MedicalRecordNumber = Guid.NewGuid();
                patient.LastName = patientViewModel.PatientViewEntity.LastName.Trim();
                patient.FirstName = patientViewModel.PatientViewEntity.FirstName.Trim();
                patient.MiddleName = patientViewModel.PatientViewEntity.MiddleName.Trim();
                patient.DOB = DateTime.Parse(patientViewModel.PatientViewEntity.DOB).Date;
                patient.Address = patientViewModel.PatientViewEntity.Address.Trim();
                patient.Phone = patientViewModel.PatientViewEntity.Phone.Trim();
                patient.Email = patientViewModel.PatientViewEntity.Email.Trim();
                patient.City = patientViewModel.PatientViewEntity.City.Trim();
                patient.State = patientViewModel.PatientViewEntity.State.Trim();
                patient.Country = patientViewModel.PatientViewEntity.Country;
                patient.IsActive = true;

                if (patientViewModel.PatientViewEntity.Gender.Equals("Male"))
                    patient.Sex = true;
                else
                    patient.Sex = false;
                               
                if (patientViewModel.PatientViewEntity.PatientId == 0)
                {
                    string defaultPassword = RandomPasswordGenerator.Generate(8);
                    defaultPassword = EncryptDecrypt.EncryptData(defaultPassword, EncryptDecrypt.ReadCert());
                    patientRepository.AddPatient(patient, patientViewModel.PatientViewEntity.UserName.Trim(), defaultPassword);
                    var sendMail = new SMTPApi(ConfigurationManager.AppSettings["SmtpFromEmailAddress"].ToString(), new List<String> { patient.Email });
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("<b>HealthReunion Patient Portal Log On Information. Below are the credentials</b><hr/>");
                    stringBuilder.AppendFormat("<br/>User Name: {0}", patientViewModel.PatientViewEntity.UserName.Trim());
                    stringBuilder.AppendFormat("<br/>Password: {0}", defaultPassword);
                    stringBuilder.AppendLine("<br/><hr/>Please log on to HealthReunion Patient Portal - http://healthreunionpatients.azurewebsites.net/ to access your clinical information.");
                    sendMail.SimpleHtmlEmail(stringBuilder.ToString());
                }
                else
                {
                    patientRepository.UpdatePatient(patient, patientViewModel.PatientViewEntity.UserName.Trim());
                }               
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(patientViewModel);
            }
            ViewBag.Message = string.Format("Patient '{0} {1} {2}' Created Sucessfully!. An Email is trigged to {3} with credentials to Log in to HealthReunion Patient Portal.", 
                patientViewModel.PatientViewEntity.FirstName, patientViewModel.PatientViewEntity.MiddleName, patientViewModel.PatientViewEntity.LastName, patientViewModel.PatientViewEntity.Email);

            patientViewModel.PatientViewEntity = new PatientViewEntity();
            patientViewModel.PatientViewModelGrid = GetPatientsForLoggedInProvider();       
            return View(patientViewModel);
        }     
    }
}
