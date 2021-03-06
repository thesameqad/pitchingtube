﻿using System;
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

        public List<PitchingTube.Data.ParticipantRepository.UserInfo> History(Guid UserId, Guid currentPartnerId, int tubeId)
        {
            var HistoryPartners = (from partner in _objectSet
                                   join p in _context.Participants on partner.PartnerId equals p.UserId
                                   where p.TubeId == tubeId && (partner.UserId == UserId || partner.UserId == currentPartnerId)
                                   select partner).ToList();
                          
            //var HistoryPartners = Query(x => x.UserId == UserId);
            //var count = HistoryPartners.Count;

            var partners = new List<ParticipantRepository.UserInfo>();
            foreach (var partner in HistoryPartners)
            {
                if(partner.UserId == currentPartnerId)
                {
                    var temp = partner.UserId;
                    partner.UserId = partner.PartnerId;
                    partner.PartnerId = temp;
                }
                

                var repository = new BaseRepository<Person>();
                var person = repository.FirstOrDefault(x => x.UserId == partner.PartnerId);
                var avatar = person.AvatarPath.Replace("\\", "/");
                var user = userPepository.FirstOrDefault(x => x.UserId == partner.PartnerId);

                var participant = participantRepository.FirstOrDefault(x => x.UserId == partner.PartnerId);

                if (partner.PartnerId != currentPartnerId)
                {
                    partners.Add(new ParticipantRepository.UserInfo()
                    {
                        Name = user.UserName,
                        AvatarPath = avatar,
                        Description = participant.Description,
                        Role = user.aspnet_Roles.FirstOrDefault().RoleName,
                        UserId = partner.UserId,
                        Contacts = partner.Contacts
                    });
                }
            }
            return partners;
        }

        public override void Insert(Partner newEntity)
        {
            aspnet_Users user = _context.aspnet_Users.FirstOrDefault(u => u.UserId == newEntity.UserId);
            int tubeId = user.Participants.FirstOrDefault().TubeId;

            var partner = (from p in _context.Partners
                           join pa in _context.Participants on p.UserId equals pa.UserId
                           join t in _context.Tubes on pa.TubeId equals t.TubeId
                           where (p.UserId == newEntity.UserId
                                  && p.PartnerId == newEntity.PartnerId
                                  && t.TubeId == tubeId)
                               || (p.UserId == newEntity.PartnerId
                                   && p.PartnerId == newEntity.UserId
                                   && t.TubeId == tubeId)
                           select p).FirstOrDefault();

            if (partner == null)
                base.Insert(newEntity);
        }

        public int GetLeftTimePitch(Guid userId, Guid partnerId, DateTime minDateTime)
        {
            var currentPair = FirstOrDefault(p => (p.UserId == userId && p.PartnerId == partnerId) || (p.UserId == partnerId && p.PartnerId == userId));

            int timeDiff = 0;
            if (currentPair.BeginPitchTime != null && currentPair.BeginPitchTime != minDateTime)
                timeDiff = Convert.ToInt32((DateTime.Now - currentPair.BeginPitchTime).Value.TotalSeconds);

            return timeDiff;

        }
    }
}
