using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Web;

namespace PitchingTube.Data
{
    partial class PitchingTubeEntities
    {
        public static PitchingTubeEntities Current
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (context.Items["PitchingTubeEntities"] == null)
                {
                    context.Items["PitchingTubeEntities"] = new PitchingTubeEntities();
                }

                return (PitchingTubeEntities)context.Items["PitchingTubeEntities"];
            }
        }
        public MailMessage GenerateEmail<T>(string templateID, T model)
        {
            MailMessage mail = new MailMessage { From = new MailAddress("thesameqad@gmail.com") };
            var template = EmailTemplates.Single(t => t.EmailTemplateID == templateID);
            mail.Subject = template.GetSubject(model);
            mail.Body = template.GetBody(model);
            mail.IsBodyHtml = true;

            return mail;
        }
    }

    public partial class Tube
    {
        public TubeMode TubeMode
        {
            get { return (TubeMode)Mode; }
            set { Mode = (int)value; }
        }
    }

    public enum TubeMode{
        Opened,
        FirstPitch,
        SecondPitch,
        ThirdPitch,
        FourthPitch,
        FifthPitch,
        Nominations,
        Closed
    }
}
