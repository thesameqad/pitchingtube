using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace PitchingTube.Data
{
    public class PartnerRepository : BaseRepository <Partner>
    {
        public class PartnerInfo
        {
            public string Name { get; set; }
            public string AvatarPath { get; set; }
            public string Contact { get; set; }
        }
        public List<PartnerInfo> History(Guid UserId)
        {
            var HistoryPartners = Query(x => x.UserId == UserId);
            var partners = new List<PartnerInfo>();
            foreach (var partner in HistoryPartners)
            {
                var repository = new BaseRepository<Person>();
                var person = repository.FirstOrDefault(x => x.UserId == partner.PartnerId);
                var avatar = person.AvatarPath.Replace("\\", "/");
                var partnerName = new BaseRepository<aspnet_Users>();
                var name = partnerName.FirstOrDefault(x => x.UserId == partner.PartnerId);
                partners.Add(new PartnerInfo() { Name = name.UserName,  AvatarPath = avatar, Contact=partner.Contacts });
            }
            return partners;
        }
    }
}
