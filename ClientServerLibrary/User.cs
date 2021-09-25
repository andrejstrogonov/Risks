using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLibrary
{
    public class AuthentificationAswer
    { 
        public bool IsKeyExists { get; set; }
        public long PeriodLeft { get; set; }
        public DateTime DateOfFinish { get; set; }
        public bool IsUsingNow { get; set; }
    }
    public class User
    {
        [Key]
        public long ID { get; set; }
        public string Login { get; set; }
        public string EmailAddress { get; set; }
        public string PasswordCode { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public string Key { get; set; }
        public DateTime DateOfStartCurrentPeriod { get; set; }
        public long CurrentPeriodDuration { get; set; }
        public bool IsUsingNow { get; set; }
        public double LongnessOfUsing { get; set; }
        public long HelpNumber { get; set; }
        public string IPAddress { get; set; }
    }

}
