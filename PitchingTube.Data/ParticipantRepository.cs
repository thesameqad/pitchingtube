using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitchingTube.Data
{
    public class ParticipantRepository: BaseRepository<Participant>
    {
        private BaseRepository<Tube> tubeRepository = new BaseRepository<Tube>();
        public int FindBestMatchingTube(string userRole)
        {
            var tubeList = from p in _objectSet
                           join aspnet_User in _context.aspnet_Users on p.UserId equals aspnet_User.UserId                           
                            where aspnet_User.aspnet_Roles.FirstOrDefault().RoleName == userRole
                            group p by p.TubeId into p
                            where p.Count() <= 4
                            orderby p.Count()
                            select p.FirstOrDefault().Tube;
            var tube = tubeList.FirstOrDefault();;

            if (tube == null)
            {
                var newTube = new Tube { 
                    CreatedDate = DateTime.Now
                    };
                tubeRepository.Insert(newTube);
                return newTube.TubeId;
            }

            return tube.TubeId;

        }

        public Tube UserIsInTube(Guid userId)
        {
            var participant = FirstOrDefault(p => p.UserId == userId);
            return participant != null?participant.Tube:null;
        }

        public void RemoveUserFromAllTubes(Guid userId)
        {
            var participants = Query(p => p.UserId == userId).ToList();
            foreach (var participant in participants)
            {
                Delete(participant);
            }

        }
    }
}
