using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace PitchingTube.Data
{
    public class ParticipantRepository : BaseRepository<Participant>
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
            var tube = tubeList.FirstOrDefault(); ;

            if (tube == null)
            {
                var newTube = new Tube
                {
                    CreatedDate = DateTime.Now,
                    TubeMode = TubeMode.Opened
                };
                tubeRepository.Insert(newTube);
                return newTube.TubeId;
            }

            return tube.TubeId;

        }

        public Tube UserIsInTube(Guid userId)
        {
            var participant = FirstOrDefault(p => p.UserId == userId);
            return participant != null ? participant.Tube : null;
        }

        public void RemoveUserFromAllTubes(Guid userId)
        {
            var participants = Query(p => p.UserId == userId).ToList();
            foreach (var participant in participants)
            {
                Delete(participant);
            }

        }
        public List<UserInfo> TubeParticipants(int tubeId)
        {
            var participants = Query(x => x.TubeId == tubeId);
            var users = new List<UserInfo>();
            foreach (var participant in participants)
            {
                var repository = new BaseRepository<Person>();
                var person = repository.FirstOrDefault(x => x.UserId == participant.UserId);
                var avatar = person.AvatarPath.Replace("\\", "/");
                var role = Roles.GetRolesForUser(participant.aspnet_Users.UserName).FirstOrDefault();
                users.Add(new UserInfo() {UserId = person.UserId, Name = participant.aspnet_Users.UserName, Role = role, Description = participant.Description, AvatarPath = avatar });
            }
            return users;
        }

        public override void Insert(Participant newEntity)
        {
            string roleName = (from u in _context.aspnet_Users
                              where u.UserId == newEntity.UserId
                              select u.aspnet_Roles.FirstOrDefault().RoleName).FirstOrDefault();

            int indexNumber = (from p in _objectSet
                              join aspnet_User in _context.aspnet_Users on p.UserId equals aspnet_User.UserId
                              where aspnet_User.aspnet_Roles.FirstOrDefault().RoleName == roleName 
                              && p.TubeId == newEntity.TubeId
                              select p).Count();
            newEntity.IndexNumber = indexNumber;
            base.Insert(newEntity);

        }

        public Participant FindPartner(Guid userId, int tubeId, int roundNumber)
        {
            string roleName = (from u in _context.aspnet_Users
                               where u.UserId == userId
                               select u.aspnet_Roles.FirstOrDefault().RoleName).FirstOrDefault();

            int indexNumber = FirstOrDefault(p => p.UserId == userId && p.TubeId == tubeId).IndexNumber ?? 0;
            //roleName = roleName == "Investor"? "Enterepreneur":"Investor";

            if (roleName == "Investor")
            {
                roleName = "Enterepreneur";
                indexNumber += roundNumber;
                indexNumber = indexNumber == 5 ? 0 : indexNumber;
            }
            else {
                roleName = "Investor";
                indexNumber -= roundNumber;
                indexNumber = indexNumber == -1 ? 4 : indexNumber;
            }

            var participant = (from p in _objectSet
                              join aspnet_User in _context.aspnet_Users on p.UserId equals aspnet_User.UserId
                              where aspnet_User.aspnet_Roles.FirstOrDefault().RoleName == roleName 
                              && p.TubeId == tubeId && p.IndexNumber == indexNumber//indexNumber+roundNumber 
                              select p).FirstOrDefault();

            return participant;

        }

        public class UserInfo
        {
            public Guid UserId { get; set; }
            public string Name { get; set; }
            public string AvatarPath { get; set; }
            public string Description { get; set; }
            public string Role { get; set; }
        }
    }
}
