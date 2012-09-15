using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace PitchingTube.Mailing
{
    public class Mailer
    {

        public static void SendMail(string to, string subject, string body)
        {
            var message = new MailMessage
            {
                Subject = subject,
                Body = body
            };
            message.To.Add(to);
            SendMail(message);
        }

        public static void SendMail(MailMessage message)
        {
            message.BodyEncoding = Encoding.UTF8;

            var client = new SmtpClient();
            client.Credentials = CredentialCache.DefaultNetworkCredentials;

            client.Send(message);
        }

    }
}
