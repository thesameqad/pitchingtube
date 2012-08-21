using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using PitchingTube.Models;
using PitchingTube.Data;
using PitchingTube.Mailing;
using System.Net.Mail;
using Facebook.Web;
using Facebook.Web.Mvc;
using System.IO;

namespace PitchingTube.Controllers
{
    public class AccountController : Controller
    {

        PersonRepository personRepository = new PersonRepository();

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(Membership.GetUserNameByEmail(model.Email), model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        [Authorize]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        //[FacebookAuthorize(LoginUrl = "/Account/Register")]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            RegisterModel model = new RegisterModel();
            model.AvatarPath = "../../Content/img/no_avatar.jpg";


            if (FacebookWebContext.Current.IsAuthenticated())
            {
                var client = new  FacebookWebClient();
                dynamic me = client.Query("select name,pic_small, email from user where uid = me()");
                
                    model.UserName = me[0].name;
                    model.Email = me[0].email;
                    model.AvatarPath = me[0].pic_small;
                
               
                return View(model);
      
            }
            
            return View(model);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model, HttpPostedFileBase fileUpload)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                try{
                    if (Membership.FindUsersByEmail(model.Email).Count != 0)
                    {
                        ModelState.AddModelError(string.Empty, "The email address is in use");
                        return View(model);
                    }
                    
                    var currentUser = Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, false, null, out createStatus);
                    Roles.AddUserToRole(model.UserName,model.Role);

                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        string activationLink = Util.GetAuthKey(Guid.Parse(currentUser.ProviderUserKey.ToString()), model.Email, model.Password);
                        var emailModel = new
                        {
                            UserName = model.UserName,
                            Url = string.Format("{0}/Account/Home/?auth={1}", Util.BaseUrl, activationLink)
                        };
                        MailMessage mail = PitchingTubeEntities.Current.GenerateEmail("activation", emailModel);
                        mail.To.Add(model.Email);

                        Mailer.SendMail(mail);

                        string avatarPath = "";
                        if (fileUpload != null)
                        {
                            string path = AppDomain.CurrentDomain.BaseDirectory + "UploadedFiles/";
                            fileUpload.SaveAs(Path.Combine(path, currentUser.ProviderUserKey.ToString()+".jpg"));
                            avatarPath = "../../UploadedFiles/" + currentUser.ProviderUserKey.ToString()+".jpg";
                        }
                        else if (!string.IsNullOrWhiteSpace(model.AvatarPath))
                        {
                            avatarPath = model.AvatarPath;
                        }
                        Person newPerson = new Person
                            {
                                Phone = model.Phone,
                                Skype = model.Skype,
                                UserId = Guid.Parse(currentUser.ProviderUserKey.ToString()),
                                ActivationLink = activationLink.Split('$')[1],
                                AvatarPath = avatarPath
                            };
                        personRepository.Insert(newPerson);
                        
                        return RedirectToAction("ActivationRequest");
                    }
                    else
                    {
                        ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    }

                }
                catch(Exception ex)
                {
                    Membership.DeleteUser(model.UserName);
                    
                    ModelState.AddModelError("", ex);
                }
            }
            

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ActivationRequest()
        {
            return View();
        }

        public ActionResult Home(string auth)
        {
            Guid userId = Guid.Parse(auth.Split('$')[0]);

            var person = personRepository.FirstOrDefault(p => p.UserId == userId);
            if (person.ActivationLink == auth.Split('$')[1])
            {
                MembershipUser user = Membership.GetUser(person.UserId);
                if (user != null)
                {
                    user.IsApproved = true;
                    Membership.UpdateUser(user);
                } 
                ViewBag.Message = "Your account has been activated. You can login now";
            }
            else
                ViewBag.Message = "That was a wrong link, or it was expired";

            return View();
            //FormsAuthentication.SetAuthCookie(person.User.UserName, false);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess
        
        [Authorize]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize]
        public ActionResult AccountSettings()
        {

            MembershipUser currentUser = Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name));
            GeneralUserModel user = new GeneralUserModel
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email
            };           
            
            return View(user);
        }

        public ActionResult FogotPassword()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public ActionResult FogotPassword(LogOnModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            try
            {
                MembershipUser currentUser = Membership.GetUser(Membership.GetUserNameByEmail(model.Email));
                var password = currentUser.ResetPassword();

                var emailModel = new
                {
                    UserName = currentUser.UserName,
                    Url = password
                };
                MailMessage mail = PitchingTubeEntities.Current.GenerateEmail("recoverpassword", emailModel);
                mail.To.Add(model.Email);

                Mailer.SendMail(mail);
                return RedirectToAction("FogotPasswordSuccess");
            }
            catch
            {
                ViewBag.Message = "Your account has not been activated. You can register now";
                return View();
            }
            
        }

        public ActionResult FogotPasswordSuccess()
        {
            return View();
        }

        #region Status Codes
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
        #endregion
    }
}
