using Newtonsoft.Json;
using StageprojectService.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StageprojectService
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            machine.Text = Methods.identifiant();
        }

       

        private async void connexion_Click(object sender, RoutedEventArgs e)
        {
            HttpClient http = new HttpClient();
            HttpResponseMessage msg = await http.GetAsync(Data.dbserver + "user/user?email="+cemail.Text+"&password="+cpasswoed.Password+"&machineName=" + Methods.identifiant());
            string webresponse = await msg.Content.ReadAsStringAsync();
            User root = null;
            try
            {

                root = JsonConvert.DeserializeObject<User>(webresponse);

                if(root.configuration != null && root.machineName != null)
                {
                    Data.config = root.configuration;
                    Data.user = new User();
                    Data.user.email = root.email;
                    Data.user.machineName = root.machineName;
                    Data.user.nom = root.nom;
                    Data.user.prenom = root.prenom;
                    MessageBox.Show("Bienvenue  " + root.prenom, "OK");
                    MainPage.mp.Hide();
                    fermer f = new fermer();
                    if(!(Service1.th.ThreadState == System.Threading.ThreadState.Running))
                    {
                        Service1.th = new Thread(new ThreadStart(Service1.getUser));
                    }
                    f.Show();
                }
                else
                {
                    throw new Exception();
                }
               

            }
            catch (Exception e1)
            {
                MessageBox.Show("il faut s'inscrire", "ERREUR");
                //Environment.Exit(0);
                //MainPage.mp.Hide();
                Console.WriteLine("User Not Found");
            }
        }

        private async void inscription_Click(object sender, RoutedEventArgs e)
        {
            if (nom.Text != String.Empty && prenom.Text != String.Empty && iemail.Text != String.Empty && ipasswoed.Password != String.Empty && icpasswoed.Password != String.Empty)
            {
                if (ipasswoed.Password == icpasswoed.Password)
                {
                    try
                    {
                        HttpClient http = new HttpClient();
                         HttpResponseMessage msg = await http.GetAsync(Data.dbserver + "user/usermachine?machineName=" + Methods.identifiant());
                        string webresponse = await msg.Content.ReadAsStringAsync();
                        User root = null;                    
                        root = JsonConvert.DeserializeObject<User>(webresponse);
                        if (root.configuration == null)
                        {
                            http = new HttpClient();
                            http.BaseAddress = new Uri("http://localhost:8080/user/save");
                            http.DefaultRequestHeaders
                            .Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            String hashpassword = BCrypt.Net.BCrypt.HashPassword(ipasswoed.Password);
                            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
                            request.Content = new StringContent("{\"machineName\":\"" + Methods.identifiant() + "\",\"prenom\":\"" + prenom.Text + "\",\"nom\":\"" + nom.Text + "\",\"email\":\"" + iemail.Text + "\",\"username\":\"" + iemail.Text + "\",\"password\":\"" + ipasswoed.Password + "\",\"configuration\":{\"idConfig\":1} }", Encoding.UTF8, "application/json");//CONTENT-TYPE header                                                                                                                                                                                                                                                                                                        //http.PostAsync(request)
                            msg = await http.SendAsync(request);
                            if (msg.IsSuccessStatusCode)
                            {
                                webresponse = await msg.Content.ReadAsStringAsync();
                                Service1.prog();
                            }
                            else
                                throw new Exception();
                        }
                        else
                        {
                            throw new Exception();
                        }


                    }
                    catch (Exception e1)
                    {
                        ipasswoed.Password = String.Empty;
                        icpasswoed.Password = String.Empty;
                        MessageBox.Show("Erreur utilisateur Existant", "ERREUR");
                        //Environment.Exit(0);
                        //MainPage.mp.Hide();
                        Console.WriteLine("User Not Found");
                    }
                }
                else
                {
                    ipasswoed.Password = String.Empty;
                    icpasswoed.Password = String.Empty;
                    MessageBox.Show("Mot de passe incorrect","ERROR");
                }
            }
            else
            {
                ipasswoed.Password = String.Empty;
                icpasswoed.Password = String.Empty;
                MessageBox.Show("Verifier Les champs", "ERROR");
            }
        }

        private void navigateconnexion_Click(object sender, RoutedEventArgs e)
        {
            if(MainPage.mp != null)
            {
                MainPage.mp.Width =(int) c.Width+20;
                MainPage.mp.Height = (int)c.Height+75;
                MainPage.mp.elementHost1.Height = MainPage.mp.Height;
                MainPage.mp.elementHost1.Width = MainPage.mp.Width;

                MainPage.mp.elementHost1.Location = new System.Drawing.Point(0, 0);

            }
            i.Visibility = Visibility.Collapsed;
            c.Visibility = Visibility.Visible;
            
        }

        private void navigateinscription_Click(object sender, RoutedEventArgs e)
        {
            if (MainPage.mp != null)
            {
                MainPage.mp.Width = (int)i.Width + 20;
                MainPage.mp.Height = (int)i.Height+75;
                MainPage.mp.elementHost1.Height = MainPage.mp.Height;
                MainPage.mp.elementHost1.Width = MainPage.mp.Width;
                MainPage.mp.elementHost1.Location = new System.Drawing.Point(0, 0);

            }
            c.Visibility = Visibility.Collapsed;
            i.Visibility = Visibility.Visible;
        }
    }
}
