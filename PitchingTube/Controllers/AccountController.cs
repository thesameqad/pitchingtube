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
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
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

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
       [FacebookAuthorize(LoginUrl = "/Account/Register")]
        public ActionResult Register()
        {
            RegisterModel model = new RegisterModel();
      

            if (FacebookWebContext.Current.IsAuthenticated())
            {
                var client = new  FacebookWebClient();
                dynamic me = client.Get("me");
                model.UserName = me.name;
                model.Email = me.email;
                model.AvatarPath = me.picture;
               
                return View(model);
      
            }
            
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {

                // Attempt to register the user
                MembershipCreateStatus createStatus;
                var currentUser = Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, false, null, out createStatus);


                /*Настя: Этот кусок кода мне не нравится, переделаю позже!*/
                string[] user = new string[1];
                user[0] = currentUser.UserName;
                /*------------------------------------------------*/

                Roles.AddUsersToRole(user, model.Role);

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


                    //проблема в том, что картинка не передается
                    //fileUpload = null всегда
                    if (fileUpload != null)
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory + "UploadedFiles/";
                        fileUpload.SaveAs(Path.Combine(path, currentUser.ProviderUserKey.ToString()));
                    }

                

                    personRepository.Insert(new Person
                    {
                        Phone = model.Phone,
                        Skype = model.Skype,
                        UserId = Guid.Parse(currentUser.ProviderUserKey.ToString()),
                        ActivationLink = activationLink.Split('$')[1]
                    });

                    return RedirectToAction("ActivationRequest");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
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

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize]
        public ActionResult AccountSettings()
        {
            
            MembershipUser currentUser = Membership.GetUser(User.Identity.Name);
            User user = new User
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email
            };           
            
            return View(user);
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
