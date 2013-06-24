using ProviderPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PatientPortal.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            var providers = new ProviderRepository().GetAllProviders();
            var loginViewModel = new LoginModel();
            loginViewModel.Providers = new Models.DrodownItemsViewModel();
            loginViewModel.Providers.Items = GetProviders();
            return View(loginViewModel);
        }
        
        private List<SelectListItem> GetProviders()
        {
            var providersList = new ProviderRepository().GetAllProviders();
            var selectedListItems = new List<SelectListItem>();

            foreach (var provider in providersList)
            {
                selectedListItems.Add(new SelectListItem { Text = provider.ProviderName, Value = provider.ProviderId.ToString() });
            }
            return selectedListItems;
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid && ValidateUser(model.UserName, model.Password, model.SelectedProvider))
                {
                    return RedirectToLocal();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            model.Providers = new Models.DrodownItemsViewModel();
            model.Providers.Items = GetProviders();
            return View(model);
        }

        private bool ValidateUser(string userName, string passWord, int selectedProvider)
        {
            try
            {
                var providers = new ProviderRepository().GetAllProviders();
                if (providers.Count > 0)
                {
                    var providerRepository = new ProviderRepository();

                    if (providerRepository.ValidateUser(userName, passWord, selectedProvider))
                    {
                        Session["UserName"] = userName;
                        Session["ProviderId"] = selectedProvider;
                        return true;
                     }
                    else
                    {
                       throw new Exception("Invalid username or password");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
       }

        public ActionResult LogOff()
        {
            Session["UserName"] = "";
            Session["ProviderId"] = "";

            return RedirectToAction("Login", "Account");
        }
        
        private ActionResult RedirectToLocal()
        {          
            return RedirectToAction("Index", "Home");        
        }    
    }
}
