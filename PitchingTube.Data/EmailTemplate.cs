using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PitchingTube.Mailing;

namespace PitchingTube.Data
{
    partial class EmailTemplate
    {
        public string GetSubject<T>(T model)
        {
            return EmailParser.Parse(EmailTemplateID, Subject, model, false, false);
        }

        public string GetBody<T>(T model)
        {
            return EmailParser.Parse(EmailTemplateID, Template, model, true, true);
        }
    }
}
