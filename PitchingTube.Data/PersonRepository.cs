using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitchingTube.Data
{
    public class PersonRepository : BaseRepository<Person>
    {

        public bool? GetPay(Guid userId)
        {
            var pay = FirstOrDefault( u => u.UserId == userId).Pay;
            return pay;
        }

        public void SetPay(Guid userId, bool pay)
        {
            var person = FirstOrDefault(u => u.UserId == userId);
            person.Pay = pay;
            Update(person);
        }

        public string GetRoleName(Guid userId)
        {
            return FirstOrDefault(p => p.UserId == userId).aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName;
        }

    }
}
