using HealthReunionDataAccess;
using ProviderPortal.Helpers;
using ProviderPortal.Models;
using System;
using System.Web.Mvc;

namespace PatientPortal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (!LogOnHelper.CheckLogOn()) RedirectToAction("Login", "Account");

            return View();
        }
    }
}
