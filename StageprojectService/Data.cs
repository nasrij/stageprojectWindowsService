using StageprojectService.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StageprojectService
{
    public class Data
    {
        public static Configuration config;
        public static User user = null;
        public static String ftpserver ="ftp://localhost/";
        public static String dbserver = "http://localhost:8080/";
        public static String ftpurl = "ftp://localhost/StageProject/src/main/resources/static/images/"; // e.g. ftp://serverip/foldername/foldername
        public static String ftpusername = "nasri"; // e.g. username
        public static String ftppassword = "123456789"; // e.g. password
    }
}
