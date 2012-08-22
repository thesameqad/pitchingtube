using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PitchingTube.Models
{

    public class UserInfo
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string AvatarPath { get; set; }
        public string Description { get; set; }
        public string Role { get; set; }
        public string Contacts { get; set; }
        public string Email { get; set; }
        public int Nomination { get; set; }
        public int Pending { get; set; }
    }
}