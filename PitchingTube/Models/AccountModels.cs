using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using PitchingTube.Data;
using System.Linq;

namespace PitchingTube.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        
        [Display(Name = "Email")]
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Your email is incorrect")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Your email is incorrect")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Skype")]
        public string Skype { get; set; }

        [Required]
        [Display(Name = "Phone")]
        [RegularExpression(@"^[\d\-]{10,}$", ErrorMessage = "Phone number is incorrect ")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        //[Required]
        [Display(Name = "Avatar")]
        public string AvatarPath
        {
            get;
            set;
        }

        public List<SelectListItem> Roles
        {
            get 
            {
                var roles = System.Web.Security.Roles.GetAllRoles();
                List<SelectListItem> rolesList = new List<SelectListItem>();

                for (int index = 0; index < roles.Length; index++)
                {
                    rolesList.Add( new SelectListItem {
                            Text = roles[index],
                            Value = roles[index]
                    });
                }

                rolesList[0].Selected = true;
                    

                return rolesList;
            }
        }
    }



}
