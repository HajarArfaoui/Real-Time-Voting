using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using VoteTempsReel.Filters;
using VoteTempsReel.Models;
using System.Web.Helpers;
using System.Data;
using System.Reflection;

namespace VoteTempsReel.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.Rem_Time = Session["Rem_Time"];
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToAction("Dashboard","Home");
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "UserName ou Mot de passe saisi est incorrecte");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            ViewBag.Rem_Time = Session["Rem_Time"];
            WebSecurity.Logout();

            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Rem_Time = Session["Rem_Time"];
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    using (UsersContext context = new UsersContext())
                    {
                        var log1 = context.UserProfiles.Where(a => a.Email.Equals(model.Email)).FirstOrDefault();
                        DateTime jour = DateTime.Now.Date;
                        DateTime j = Convert.ToDateTime(model.Naissance);
                        TimeSpan ts = jour - j;
                        int differenceInDays = ts.Days;
                        int nbr = differenceInDays / 365;
                        if (nbr >= 18 && log1 == null)
                        {
                            if (file != null)
                            {
                                
                                WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                                UserProfile user = context.UserProfiles.FirstOrDefault(u => u.UserName == model.UserName);
                                string ImageName = System.IO.Path.GetFileName(file.FileName);
                                string physicalPath = Server.MapPath("~/images/" + ImageName);
                                file.SaveAs(physicalPath);
                                user.Nom = model.Name;
                                user.Prenom = model.Prenom;
                                user.CIN = model.CIN;
                                user.Adresse = model.Adresse;
                                user.Date = model.Naissance;
                                user.Telephone = model.Telephone;
                                user.Email = model.Email;
                                user.Password = model.Password;
                                user.Voter = "0";
                               
                                user.Url_image = ImageName;
                                context.SaveChanges();
                                WebSecurity.Login(model.UserName, model.Password);

                            }
                            else
                            {
                                WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                                UserProfile user = context.UserProfiles.FirstOrDefault(u => u.UserName == model.UserName);
                                /*string physicalPath = Server.MapPath("~/Images/"+"default.png");
                                file.SaveAs(physicalPath);*/
                                string ImageName = System.IO.Path.GetFileName("default.png");
                                string physicalPath = Server.MapPath("~/images/" + ImageName);
                                user.Nom = model.Name;
                                user.Prenom = model.Prenom;
                                user.CIN = model.CIN;
                                user.Adresse = model.Adresse;
                                user.Date = model.Naissance;
                                user.Telephone = model.Telephone;
                                user.Email = model.Email;
                                user.Password = model.Password;
                                user.Voter = "0";
                                user.Url_image = "default.png";
                                context.SaveChanges();

                                WebSecurity.Login(model.UserName, model.Password);
                               
                                
                            }
                            Roles.AddUserToRole(model.UserName, "user");
                           
                            
                            return RedirectToAction("../Home/Dashboard");
                        }
                        else
                        {
                            if (nbr < 18)
                            {
                                ModelState.AddModelError("", "le vote est limité aux personnes qui ont au moins 18 ans");
                            }
                            if (log1 != null)
                            {
                                ModelState.AddModelError("", "Email déja existe . Saissir un autre Email");
                            }

                        }

                    }
                    return View();



                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Display()
        {
            return View(context.UserProfiles.ToList());
        }

        UsersContext context = new UsersContext();
        [Authorize]
        public ActionResult List()
        {
           
                UserProfile t = context.UserProfiles.First(x => x.UserName == User.Identity.Name);
                int id = t.UserId;
                if (Roles.GetRolesForUser(User.Identity.Name).Contains("admin"))
                {
                    ViewBag.Rem_Time = Session["Rem_Time"];

                    return View(context.UserProfiles.ToList());
                }
                else
                {
                    return RedirectToAction("NotFound", "Error");
                }
        }

        [Authorize(Roles = "admin")]
        public ActionResult Details(int UserId = 0)
        {
            ViewBag.Rem_Time = Session["Rem_Time"];
           
           
            return View(context.UserProfiles.Find(UserId));
        }
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int UserId = 0)
        {
            ViewBag.Rem_Time = Session["Rem_Time"];
            return View(context.UserProfiles.Find(UserId));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(UserProfile e)
        {
            context.Entry(e).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("List");

        }
        public void DeleteUser(int id)
        {
            var tmpuser = "";
            var ctx = new UsersContext();
            using (ctx)
            {
                var firstOrDefault = ctx.UserProfiles.FirstOrDefault(us => us.UserId == id);
                if (firstOrDefault != null)
                    tmpuser = firstOrDefault.UserName;
            }

            string[] allRoles = Roles.GetRolesForUser(tmpuser);
            Roles.RemoveUserFromRoles(tmpuser, allRoles);
            ctx = new UsersContext();
           

          
        }
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int UserId = 0)
        {
            ViewBag.Rem_Time = Session["Rem_Time"];
            return View(context.UserProfiles.Find(UserId));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult Delete_conf(int UserId)
        {
            DeleteUser(UserId);
            UserProfile e = context.UserProfiles.Find(UserId);
            context.UserProfiles.Remove(e);
            context.SaveChanges();
            return RedirectToAction("List");

        }
        [Authorize(Roles="admin")]
        public ActionResult RoleAddToUser()
        {
            UsersContext dbContext = new UsersContext();
            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> items = dbContext.UserProfiles.Select(c => new SelectListItem
            {
                Value = c.UserName,
                Text = c.UserName

            });
            ViewBag.JobTitle = items;
            SelectList list = new SelectList(Roles.GetAllRoles());
            ViewBag.Roles = list; 
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string RoleName, string JobTitle)
        {
            UsersContext con = new UsersContext();
            /*if (Roles.IsUserInRole(JobTitle, RoleName))
            {
                ViewBag.ResultMessage = "Ce personne a déjà ce rôle";
                return View();
            }*/
            UserProfile t = con.UserProfiles.First(x => x.UserName == JobTitle);
                int id = t.UserId;
                DeleteUser(id);
                Roles.AddUserToRole(JobTitle, RoleName);
           /* SelectList list = new SelectList(Roles.GetAllRoles());
            ViewBag.Roles = list;*/
            return RedirectToAction("List");
        }

    


        [AllowAnonymous]
        public ActionResult RecupAccount()
        {
            ViewBag.Rem_Time = Session["Rem_Time"];
            return View(new RecupAccountModel());
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RecupAccount(RecupAccountModel model)
        {
            // On vérifie que les deux champs sont remplis
            if (ModelState.IsValid)
            {
                using (UsersContext rs = new UsersContext())
                {

                    //var log = rs.Enregistrer.Where(a => a.UserName.Equals(reg.Email)).FirstOrDefault();
                    var log = rs.UserProfiles.Where(a => a.Email.Equals(model.Email)).FirstOrDefault();
                    if (log != null)
                    {
                        WebMail.Send(model.Email

                            , "Password",
                            "your password is  " + log.Password,
                            null,
                            null,
                            null,
                            true,
                            null,
                            null,
                            null,
                            null,
                            null,
                            model.Email);

                        ModelState.AddModelError("", "Merci de vérifier votre boite de messagerie.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email ne correspond à aucun compte.");
                    }
                }
            }
            
            return View();
        }

         [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();

        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Contact(Contact model)
        {
            if (ModelState.IsValid)
            {
                using (UsersContext rs = new UsersContext())
                {

                    //var log = rs.Enregistrer.Where(a => a.UserName.Equals(reg.Email)).FirstOrDefault();
                   
                        WebMail.Send("hajar.arfaoui@usmba.ac.ma"

                            , model.Sujet,
                            model.Message ,
                            model.Email,
                            null,
                            null,
                            true,
                            null,
                            null,
                            null,
                            null,
                            null,
                            model.Email);

                        ModelState.AddModelError("", "Nous vous remercions pour votre contact .");
                    
                    
                }
            }

            return View();
        }
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
     
        //
        // POST: /Account/Disassociate

       /* [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    DateTime jour = DateTime.Now.Date;
                    DateTime j = Convert.ToDateTime(model.Naissance);
                    TimeSpan ts = jour - j;
                    int differenceInDays = ts.Days;
                    int nbr = differenceInDays / 365;
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

                    if (nbr >= 18 && user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }

                    else
                    {

                        ModelState.AddModelError("UserName", "UserName déja existe. Entrer un autre UserName ");


                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }*/

       
        //#endregion
        
    }
}
