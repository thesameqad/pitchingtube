using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PitchingTube.Models
{
    public class GeneralUserModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public Guid UserID { get; set; }
        public string Description { get; set; }
        public string AvatarPath { get; set; }
    }
}