using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBDotCom.Models
{
    public class EmailServerConfiguration
    {
        public EmailServerConfiguration(int _smtpPort = 587)
        {
            SmtpPort = _smtpPort;
        }

        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }

    }
}
