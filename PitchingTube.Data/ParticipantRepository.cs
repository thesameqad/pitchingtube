﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Data.SqlClient;
using System.Data;

namespace PitchingTube.Data
{
    public class ParticipantRepository : BaseRepository<Participant>
    {
        private BaseRepository<Tube> tubeRepository = new BaseRepository<Tube>();


        private int findTube(string userRole)
        {
            int tubeId = 0;
            try
            {
                SqlConnectionStringBuilder connectionstring = new SqlConnectionStringBuilder();
                connectionstring.DataSource = @".\SQLEXPRESS";
                connectionstring.InitialCatalog = "PitchingTube";
                connectionstring.IntegratedSecurity = true;

                SqlConnection connection = new SqlConnection(connectionstring.ConnectionString);
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "findTube";

                SqlParameter param = new SqlParameter();
                param.ParameterName = "UserRole";
                param.Value = userRole;

                command.Parameters.Add(param);
                command.CommandType = CommandType.StoredProcedure;


                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataSet ds = new DataSet();
                adapter.Fill(ds);

                tubeId = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            return tubeId;
        }

        public int FindBestMatchingTube(string userRole)
        {
            //var tubeList = from p in _objectSet
            //               join aspnet_User in _context.aspnet_Users on p.UserId equals aspnet_User.UserId
            //               where aspnet_User.aspnet_Roles.FirstOrDefault().RoleName == userRole
            //               group p by p.TubeId into p
            //               where p.Count() <= 4
            //               orderby p.Count() descending
            //               select p.FirstOrDefault().Tube;

            //var tube = tubes.FirstOrDefault();

            var tube = findTube(userRole);

            if (tube == 0)//null)
            {
                var newTube = new Tube
                {
                    CreatedDate = DateTime.Now,
                    TubeMode = TubeMode.Opened
                };
                tubeRepository.Insert(newTube);
                return newTube.TubeId;
            }

            return tube;//.TubeId;

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
                roleName = "Entrepreneur";
                indexNumber += roundNumber;
                indexNumber = indexNumber >= 5 ? indexNumber - 5 : indexNumber;
            }
            else {
                roleName = "Investor";
                indexNumber -= roundNumber;
                indexNumber = indexNumber <= -1 ? indexNumber + 5 : indexNumber;
            }

            var participant = (from p in _objectSet
                              join aspnet_User in _context.aspnet_Users on p.UserId equals aspnet_User.UserId
                              where aspnet_User.aspnet_Roles.FirstOrDefault().RoleName == roleName 
                              && p.TubeId == tubeId && p.IndexNumber == indexNumber//indexNumber+roundNumber 
                              select p).FirstOrDefault();
          
            return participant;

        }

        public List<UserInfo> FindCurrentPairs(int tubeId, int roundNumber)
        {
            List<UserInfo> currentPairs = new List<UserInfo>();

            var tube = (from p in _objectSet
                        join aspnet_User in _context.aspnet_Users on p.UserId equals aspnet_User.UserId
                        join person in _context.Persons on aspnet_User.UserId equals person.UserId
                        where p.TubeId == tubeId
                        select new
                        {
                            UserId = aspnet_User.UserId,
                            UserName = aspnet_User.UserName,
                            AvatarPath = person.AvatarPath,
                            IndexNumber = p.IndexNumber
                        });

            var investors = from t in tube
                            join aspnet_User in _context.aspnet_Users on t.UserId equals aspnet_User.UserId
                            where aspnet_User.aspnet_Roles.FirstOrDefault().RoleName == "Investor"
                            select t;

            var entrepreneurs = from t in tube
                                join aspnet_User in _context.aspnet_Users on t.UserId equals aspnet_User.UserId
                                where aspnet_User.aspnet_Roles.FirstOrDefault().RoleName == "Entrepreneur"
                                select t;

            foreach (var inv in investors)
            {
                currentPairs.Add(new UserInfo
                {
                    UserId = inv.UserId,
                    Name = inv.UserName,
                    AvatarPath = inv.AvatarPath
                });

                int indexNumber = (int)inv.IndexNumber + roundNumber;

                var hisPair = (from e in entrepreneurs
                               where e.IndexNumber == (indexNumber >= 5 ? indexNumber - 5 : indexNumber)
                               select e).FirstOrDefault();

                currentPairs.Add(new UserInfo
                {
                    UserId = hisPair.UserId,
                    Name = hisPair.UserName,
                    AvatarPath = hisPair.AvatarPath
                });
            }

            return currentPairs;
        }

        public class UserInfo
        {
            public Guid UserId { get; set; }
            public string Name { get; set; }
            public string AvatarPath { get; set; }
            public string Description { get; set; }
            public string Role { get; set; }
            public string Contacts { get; set; }
        }
    }
}
