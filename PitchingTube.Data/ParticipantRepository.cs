using System;
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

        public int FindBestMatchingTube(string userRole, bool autoFillingEnabled)
        {
             

            if (!autoFillingEnabled)
                return FindBestMatchingTube(userRole);
            // auto filling
            int investorsCount = userRole == "Investor" ? 3 : 4;
            int entrepreneursCount = userRole == "Entrepreneur" ? 3 : 4;

            // 1. Find 4 investors and 3 entrepreneurs

            var investorsList = (from p in _context.Persons
                           join aspnet_User in _context.aspnet_Users on p.UserId equals aspnet_User.UserId
                           //join pa in _context.Participants on aspnet_User.UserId equals pa.UserId
                           where aspnet_User.aspnet_Roles.FirstOrDefault().RoleName == "Investor"
                           && p.IsBot == true && !aspnet_User.Participants.Any()
                           select p).ToList();

            var entrepreneursList = (from p in _context.Persons
                                 join aspnet_User in _context.aspnet_Users on p.UserId equals aspnet_User.UserId
                                 //join pa in _context.Participants on aspnet_User.UserId equals pa.UserId
                                 where aspnet_User.aspnet_Roles.FirstOrDefault().RoleName == "Entrepreneur"
                                 && p.IsBot == true && !aspnet_User.Participants.Any()
                                 select p).ToList();

            if(investorsList.Count < 4 || entrepreneursList.Count < 3 )
                return FindBestMatchingTube(userRole);


            // 2. Create new tube
            var newTube = new Tube
            {
                CreatedDate = DateTime.Now,
                TubeMode = TubeMode.Opened
            };
            tubeRepository.Insert(newTube);

            List<string> investorsDescriptions = new List<string>
            {
                "Early stage investments in web startups",
                "Social startups",
                "Great ideas ...",
                "Augment Reality please",
                "Anything but location based",
                "Seeking for inspiration",
                "Solar enegry please",
                "Something fun",
                "Big and ambitiuos",
                "Needs for the minimum 3x extit",
                "Small and easy"
            };

            Random rand = new Random();
            //3. Adding them to the tube
            foreach(var investor in investorsList.Take(investorsCount))
            {
                int randomIndex = rand.Next(0,investorsCount);
                string description = investorsDescriptions[randomIndex];
                investorsDescriptions.RemoveAt(randomIndex);
                Insert(new Participant
                {
                    TubeId = newTube.TubeId,
                    UserId = investor.UserId,
                    Description = description
                });
            }

            List<string> entrepreneursDescriptions = new List<string>
            {
                "Startup for investors",
                "Salers and ships idea",
                "Online shop",
                "Ebay-like startup",
                "Connections tree idea",
                "Airtime copycat in India",
                "Automobile petrol exchange",
                "Social network for writers ",
                "Green startup",
                "Alcohol-metr",
                "Prague stakehouse reviews",
                "Social game",
                "Seeking for the party startup"
            };

            foreach (var entrepreneur in entrepreneursList.Take(entrepreneursCount))
            {
                int randomIndex = rand.Next(0, entrepreneursCount);
                string description = entrepreneursDescriptions[randomIndex];
                entrepreneursDescriptions.RemoveAt(randomIndex);
                Insert(new Participant
                {
                    TubeId = newTube.TubeId,
                    UserId = entrepreneur.UserId,
                    Description = description
                });
            }


            return newTube.TubeId;
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

            if (tube == 0) //null)
            {
                var newTube = new Tube
                    {
                        CreatedDate = DateTime.Now,
                        TubeMode = TubeMode.Opened
                    };
                tubeRepository.Insert(newTube); 
                return newTube.TubeId;
            }

            return tube; //.TubeId;

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
                users.Add(new UserInfo()
                    {
                        UserId = person.UserId,
                        Name = participant.aspnet_Users.UserName,
                        Role = role,
                        Description = participant.Description,
                        AvatarPath = avatar
                    });
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
            if(FirstOrDefault(p => p.UserId == newEntity.UserId && p.TubeId == newEntity.TubeId) == null)
                base.Insert(newEntity);

        }

        public Participant FindPartner(Guid userId, int tubeId, int roundNumber)
        {

            int countPairs = CountPairsInTube(tubeId);

            string roleName = (from u in _context.aspnet_Users
                               where u.UserId == userId
                               select u.aspnet_Roles.FirstOrDefault().RoleName).FirstOrDefault();

            int indexNumber = FirstOrDefault(p => p.UserId == userId && p.TubeId == tubeId).IndexNumber ?? 0;
            //roleName = roleName == "Investor"? "Enterepreneur":"Investor";

            if (roleName == "Investor")
            {
                roleName = "Entrepreneur";
                indexNumber += roundNumber;
                indexNumber = indexNumber >= countPairs ? indexNumber - countPairs : indexNumber;
            }
            else
            {
                roleName = "Investor";
                indexNumber -= roundNumber;
                indexNumber = indexNumber <= -1 ? indexNumber + countPairs : indexNumber;
            }

            var participant = (from p in _objectSet
                               join aspnet_User in _context.aspnet_Users on p.UserId equals aspnet_User.UserId
                               where aspnet_User.aspnet_Roles.FirstOrDefault().RoleName == roleName
                                     && p.TubeId == tubeId && p.IndexNumber == indexNumber
                               //indexNumber+roundNumber 
                               select p).FirstOrDefault();

            return participant;

        }

        public List<UserInfo> FindCurrentPairs(Guid userId, Guid partnerId, int tubeId, int roundNumber)
        {
            int countPairs = CountPairsInTube(tubeId);

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
                            where aspnet_User.aspnet_Roles.FirstOrDefault().RoleName == "Investor" && aspnet_User.UserId != userId && aspnet_User.UserId != partnerId
                            select t;

            var entrepreneurs = from t in tube
                                join aspnet_User in _context.aspnet_Users on t.UserId equals aspnet_User.UserId
                                where aspnet_User.aspnet_Roles.FirstOrDefault().RoleName == "Entrepreneur" && aspnet_User.UserId != userId && aspnet_User.UserId != partnerId
                                select t;

            foreach (var inv in investors)
            {

                currentPairs.Add(new UserInfo
                    {
                        UserId = inv.UserId,
                        Name = inv.UserName,
                        AvatarPath = inv.AvatarPath
                    });

                int indexNumber = (int) inv.IndexNumber + roundNumber;

                var hisPair = (from e in entrepreneurs
                               where e.IndexNumber == (indexNumber >= countPairs ? indexNumber - countPairs : indexNumber)
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
        public string GetPay(Guid userId)
        {
            var pay = (from u in _context.Persons
                       where u.UserId == userId
                       select u.Pay).FirstOrDefault();
            return pay;
           // return null;
        }

        public class UserInfo
        {
            public Guid UserId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string AvatarPath { get; set; }
            public string Description { get; set; }
            public string Role { get; set; }
            public int Nomination { get; set; }
            public int Pending { get; set; }
            public string Contacts { get; set; }
        }

        public List<UserInfo> GetResult(int tubeId)
        {
            var repository = new BaseRepository<Nomination>();
            var repositoryP = new BaseRepository<Person>();
            var list = new List<UserInfo>();
            var enterepreneursId = repository.Query(
                x =>
                x.TubeId == tubeId && x.Panding == 1 &&
                x.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName == "Entrepreneur").Select(x => x.EnterepreneurId).
                Distinct();
            foreach (var enterepreneurId in enterepreneursId)
            {
                var model =
                    repository.Query(x => x.TubeId == tubeId && x.EnterepreneurId == enterepreneurId && x.Panding == 1).
                        Select(x => new UserInfo()
                            {
                                UserId = x.EnterepreneurId,
                                Name = x.aspnet_Users.UserName,
                                Description =
                                    new ParticipantRepository().FirstOrDefault(
                                        z => z.TubeId == tubeId && z.UserId == enterepreneurId).Description,
                                Nomination =
                                    repository.Query(
                                        y =>
                                        y.TubeId == tubeId && y.EnterepreneurId == enterepreneurId && x.Panding == 1).
                                        Sum(y => y.Rating),
                                Role = x.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName,
                                AvatarPath =
                                    repositoryP.FirstOrDefault(y => y.UserId == enterepreneurId).AvatarPath.Replace(
                                        "\\", "/"),
                            }).FirstOrDefault();
                list.Add(model);
            }

            var investors =
                Query(x => x.TubeId == tubeId && x.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName == "Investor").
                    Select(x => x.UserId);
            foreach (var investor in investors)
            {
                var model = new ParticipantRepository().Query(x => x.TubeId == tubeId && x.UserId == investor).Select(
                    x => new UserInfo()
                        {
                            UserId = x.UserId,
                            Role = x.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName,
                            Name = x.aspnet_Users.UserName,
                            AvatarPath =
                                repositoryP.FirstOrDefault(y => y.UserId == investor).AvatarPath.Replace("\\", "/"),
                            Pending =
                                repository.Query(z => z.TubeId == tubeId && z.InvestorId == x.UserId).Select(
                                    z => z.Panding).FirstOrDefault()
                        }).FirstOrDefault();
                list.Add(model);
            }
            return list;
        }

        public object getNominationAndPendingStatus(int tubeId)
        {
            IEnumerable<UserInfo> results = GetResult(tubeId);

            object data = new
                       {
                           nomination = results.Where(p => p.Role == "Entrepreneur").Select(p => p.Nomination),
                           pending = results.Where(p => p.Role == "Investor").Select(p => p.Pending)
                       };
            return data;
        }

        public bool IsCanFindPartner(Guid userId, int tubeId)
        {
            var participant = FirstOrDefault(p => p.UserId == userId && p.TubeId == tubeId);

            return FirstOrDefault(p => p.TubeId == tubeId && p.UserId != userId && p.IndexNumber == participant.IndexNumber) != null;
        }

        public int CountPairsInTube(int tubeId)
        {
            var count = (from p in _objectSet
                          where p.TubeId == tubeId
                          group p by p.IndexNumber
                          into p
                          where p.Count() == 2
                          select p).Count();
            
            return count;
        }

        public void DeleteFromTubeSingleUsers(int tubeId)
        {
            var singleUsers = (from p in _objectSet
                              where p.TubeId == tubeId
                              group p by p.IndexNumber
                              into p
                              where p.Count() == 1
                              select p).ToList();

            //for (int i = 0; i < singleUsers.Count; i++)
            //{
            //    Delete(singleUsers[i].FirstOrDefault());
            //}

            foreach (var user in singleUsers)
            {                
                Delete(user.FirstOrDefault());
            }
        }
    }
}
