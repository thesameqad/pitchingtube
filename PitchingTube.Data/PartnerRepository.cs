using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace PitchingTube.Data
{
    public class PartnerRepository : BaseRepository <Partner>
    {
        private ParticipantRepository participantRepository = new ParticipantRepository();
        private BaseRepository<aspnet_Users> userPepository = new BaseRepository<aspnet_Users>();

        public List<PitchingTube.Data.ParticipantRepository.UserInfo> History(Guid UserId)
        {
            var HistoryPartners = Query(x => x.UserId == UserId);
            var partners = new List<PitchingTube.Data.ParticipantRepository.UserInfo>();
            foreach (var partner in HistoryPartners)
            {
                var repository = new BaseRepository<Person>();
                var person = repository.FirstOrDefault(x => x.UserId == partner.PartnerId);
                var avatar = person.AvatarPath.Replace("\\", "/");
                var user = userPepository.FirstOrDefault(x => x.UserId == partner.PartnerId);

                var participant = participantRepository.FirstOrDefault(x => x.UserId == partner.PartnerId);

                partners.Add(new PitchingTube.Data.ParticipantRepository.UserInfo() { 
                    Name = user.UserName,  
                    AvatarPath = avatar,
                    Description = participant.Description,
                    Role = user.aspnet_Roles.FirstOrDefault().RoleName,
                    UserId = partner.UserId,
                    Contacts=partner.Contacts });
            }
            return partners;
        }
    }
}
