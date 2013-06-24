using HealthReunionDataAccess;
using ProviderPortal.Helpers;
using ProviderPortal.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ProviderPortal.Controllers
{
    public class ProviderController : Controller
    {
        public ActionResult ProviderRegistration()
        {
             return View(new ProviderViewModel());
        }

        public ActionResult NewUser()
        {
            if (!LogOnHelper.CheckLogOn()) return RedirectToAction("Login", "Account");

            var userViewModel = new UserViewModel();
            userViewModel.UserViewEntity.ProviderId = int.Parse(Session["ProviderId"].ToString());
            userViewModel.UserViewEntityGrid = GetAllUsers();
            return View(userViewModel);
        }

        private List<UserViewEnity> GetAllUsers()
        {
            var userRepository = new UserRepository();
            List<UserViewEnity> usersList = new List<UserViewEnity>();
            var users = userRepository.GetUsersByProviderId(int.Parse(Session["ProviderId"].ToString()));
            foreach (var user in users)
            {
                usersList.Add(new UserViewEnity
                {
                    UserId = user.UserId,
                    UserName = user.UserName                    
                });
            }
            return usersList;
        }

        [HttpPost]
        public ActionResult RemoveUser(int userId)
        {
            var userRepository = new UserRepository();
            userRepository.RemoveUser(userId);
            var userViewModel = new UserViewModel();
            userViewModel.UserViewEntityGrid = GetAllUsers();
            return PartialView("UserGrid", userViewModel);
        }

        [HttpPost]
        public ActionResult NewUser(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                userViewModel.UserViewEntity.ProviderId = int.Parse(Session["ProviderId"].ToString());
                userViewModel.UserViewEntityGrid = GetAllUsers();
                return View(userViewModel);
            }
            try
            {
                var userRepository = new UserRepository();
                userRepository.AddUser(new User
                {
                    UserName = userViewModel.UserViewEntity.UserName,
                    Password = EncryptDecrypt.EncryptData(userViewModel.UserViewEntity.Password, EncryptDecrypt.ReadCert()),
                    ProviderId = userViewModel.UserViewEntity.ProviderId 
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(userViewModel);
            }
            ViewBag.Message = "Created Successfully!";
            return RedirectToAction("NewUser");
        }

        [HttpPost]
        public ActionResult ProviderRegistration(ProviderViewModel providerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(providerViewModel);
            }
            try
            {
                HealthReunionDataAccess.HealthReunionEntities dataContext = new HealthReunionDataAccess.HealthReunionEntities();
                ProviderRepository providerRepository = new ProviderRepository();

                var provider = new Provider
                {
                    ProviderName = providerViewModel.ProviderName,
                    TermsOfUse = providerViewModel.TermsOfUse.Trim(),
                    ProviderDescription = providerViewModel.ProviderDescription.Trim(),
                    PrivacyStatement = providerViewModel.PrivacyStatement.Trim(),
                    AuthorizationReason = providerViewModel.AuthorizationReason.Trim(),
                    Email = providerViewModel.Email.Trim()
                };

                var user = new User
                {
                    UserName = providerViewModel.UserName.Trim(),
                    Password = providerViewModel.Password.Trim(),
                    ProviderId = provider.ProviderId
                };

                providerRepository.AddProviderWithDefaultUser(provider, user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(providerViewModel);
            }
            ViewBag.Message = string.Format("Provider '{0}' Created Sucessfully!", providerViewModel.ProviderName);
            return View(new ProviderViewModel());
        }        
    }
}
