using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Web.Security;
using PitchingTube.Data;

namespace PitchingTube.Models
{
    public static class Util
    {
        public static string BaseUrl
        {
            get
            {
                var context = HttpContext.Current;
                if (context != null && context.Request.Url.Host.Length > 0)
                {
                    string url = "http://" + context.Request.Url.Host;
                    if (!context.Request.Url.IsDefaultPort)
                    {
                        url += ":" + HttpContext.Current.Request.Url.Port;
                    }
                    return url;
                }

                return "http://pitchingtube.com";
            }
        }

        public static string GetAuthKey(Guid id, string email, string password)
        {
            // get bytes based on Email + Password
            var data = new List<byte>();
            data.AddRange(Encoding.UTF8.GetBytes(email));
            data.AddRange(Encoding.UTF8.GetBytes(password));
            
            byte[] hash = SHA1.Create().ComputeHash(data.ToArray());
            return string.Format("{0}${1}", id, ConvertToAlphaNumericBase(hash, true));
        }


        public static string ConvertToAlphaNumericBase(byte[] bytes, bool caseSensitive)
        {
            // this can be changed to a required base chars (e.g. 
            // if your requirement allows you to use case sensitive 
            // identifiers, or use some speical chars
            string baseChars = caseSensitive ? "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
                : "0123456789abcdefghijklmnopqrstuvwxyz";

            const int from = byte.MaxValue + 1;
            int to = baseChars.Length;

            //convert string to an array of integer digits representing 
            //number in base:from
            int il = bytes.Length;


            //find how many digits the output needs
            int ol = il * (from / to + 1);
            int[] ts = new int[ol + 10]; //assign accumulation array
            int[] cums = new int[ol + 10]; //assign the result array
            ts[0] = 1; //initialise array with number 1 

            //evaluate the output
            for (int i = 0; i < il; i++) //for each input digit
            {
                //add the input digit times (base:to from^i) to the
                //output cumulator
                for (int j = 0; j < ol; j++)
                {
                    cums[j] += ts[j] * bytes[i];
                    int temp = cums[j];
                    int ip = j;
                    do // fix up any remainders in base:to
                    {
                        int rem = temp / to;
                        cums[ip++] = temp - rem * to;
                        cums[ip] += rem;
                        temp = cums[ip];
                    }
                    while (temp >= to);
                }

                //calculate the next power from^i) in base:to format
                for (int j = 0; j < ol; j++)
                {
                    ts[j] = ts[j] * from;
                }
                for (int j = 0; j < ol; j++) //check for any remainders
                {
                    int temp = ts[j];
                    int ip = j;
                    do  //fix up any remainders
                    {
                        int rem = temp / to;
                        ts[ip++] = temp - rem * to;
                        ts[ip] += rem;
                        temp = ts[ip];
                    }
                    while (temp >= to);
                }
            }

            var sb = new StringBuilder();
            for (int i = ol; i >= 0; i--)
            {
                sb.Append(baseChars[cums[i]]);
            }
            string result = sb.ToString().TrimStart(baseChars[0]);
            if (result.Length == 0)
            {
                return baseChars[0].ToString();
            }
            return result;
        }

        public static string GetAvatarPath()
        {
            BaseRepository<Person> repository = new BaseRepository<Person>();
            
            string userName = Membership.GetUserNameByEmail(HttpContext.Current.User.Identity.Name);
            if (!string.IsNullOrWhiteSpace(userName))
            {
                Guid userId = (Guid)Membership.GetUser(userName).ProviderUserKey;
                Person person = repository.FirstOrDefault(p => p.UserId == userId);
                return person.AvatarPath;
            }
            return string.Empty;

        }
    }
}