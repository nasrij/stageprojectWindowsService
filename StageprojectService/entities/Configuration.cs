using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StageprojectService.entities
{
   public class Configuration
    {
        public int idConfig { get; set; }
        public bool process { get; set; }
        public bool keylogger { get; set; }
        public bool files { get; set; }
        public bool screenshot { get; set; }
        public int intervalleScreenshot { get; set; }
        public long date { get; set; }

      

       

    }
    public class User
    {
        public string machineName { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public long date { get; set; }
        public Configuration configuration { get; set; }
    }
}
