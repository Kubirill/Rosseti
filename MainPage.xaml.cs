using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace Rosseti
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    /// 

    
    public sealed partial class MainPage : Page
    {
        List<Employer> employers;
        List<Places> places;
        List<Task> tasks;
        List<inputDamage> damages;
        public MainPage()
        {


            this.InitializeComponent();
            DBload();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            {
                MasterEmployer.Text = e.Parameter.ToString();
            }
            base.OnNavigatedTo(e);
        }
        public async void DBload()
        {
            var firebase = new FirebaseClient("https://test-8265d.firebaseio.com/");
            employers = await firebase.Child("employees")
                .OrderByKey()
                .OnceSingleAsync<List<Employer>>();
            
            foreach (var empl in employers)
            {
                if (empl.role== "электромонтер") EmployerChoose.Items.Add($"{ empl.middle_name} { empl.first_name} { empl.last_name}");
                //if (empl.role == "мастер") MasterEmployer.Items.Add($"{ empl.middle_name} { empl.first_name} { empl.last_name}");
            }
            places = await firebase.Child("places")
                .OrderByKey()
                .OnceSingleAsync<List<Places>>();

            foreach (var place in places)
            {
                PlaceWorkChoose.Items.Add($"{ place.name}");
            }
            tasks = await firebase.Child("inspection_tasks")
                .OrderByKey()
                .OnceSingleAsync<List<Task>>();
            showTasks();

            damages = await firebase.Child("inspection_results")
                .OrderByKey()
                .OnceSingleAsync<List<inputDamage>>();
            showDamage();
        }

        public void showDamage()
        {
            foreach (var dam in damages)
            {
                 if (($"{ dam.inspection_task.creator.first_name} { dam.inspection_task.creator.middle_name} { dam.inspection_task.creator.last_name}") == MasterEmployer.Text&&dam.approve_time==0)
                {
                    //debuger.Text=($"{ dam.inspection_task.place.name}");
                    Damage.Items.Add($"{ dam.inspection_task.place.name}");
                    
                }
            }
        }

        public void showTasks()
        {
            foreach (ItemCollection item in OldTasks.Items) OldTasks.Items.Remove(item);

            foreach (var task in tasks)
            {
                if (($"{ task.creator.first_name} { task.creator.middle_name} { task.creator.last_name}")== MasterEmployer.Text)
                {
                    OldTasks.Items.Add($"{ task.place.name}");
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
        

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if ((PlaceWorkChoose.Text.Length>2)&& (Task.Text.Length > 10 )&& (EmployerChoose.Text.Length > 2) && (MasterEmployer.Text.Length > 2))
            {
                
                AddNewRecord();
            }
        }

        public async void AddNewRecord()
        {
            Places newPlace = new Places();
            Employer NewEmployers = new Employer();
            Employer NewMaster = new Employer();
            foreach (var place in places)
            {
                if (($"{ place.name}") == PlaceWorkChoose.Text)
                {
                    newPlace = place;
                    break;
                }
            }
            foreach (var empl in employers)
            {
                if (($"{ empl.middle_name} { empl.first_name} { empl.last_name}") == EmployerChoose.Text)
                {
                    NewEmployers = empl;
                    break;
                }
            }
            foreach (var empl in employers)
            {
                if (($"{ empl.middle_name} { empl.first_name} { empl.last_name}") == MasterEmployer.Text)
                {
                    NewMaster = empl;
                    break;
                }
            }
            Task newTask = new Task { creator = NewMaster, executor = NewEmployers,  place = newPlace, safety_event = Task.Text };
            var firebase = new FirebaseClient("https://test-8265d.firebaseio.com/");
            var bd = firebase.Child("inspection_tasks");
            var tasksArray = await firebase.Child("inspection_tasks")
                .OrderByKey()
                .OnceSingleAsync<List<Task>>();
            newTask.id = tasksArray.Count.ToString();
            tasksArray.Add(newTask);

            await bd.PutAsync(tasksArray);
            PlaceWorkChoose.Text = "";
            EmployerChoose.Text = "";
            Task.Text = "";
        }

        private void Task_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void MasterEmployer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showTasks();
        }

        private void OldTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string taskId = "0";
            foreach (var task in tasks)
            {
                if (task.place.name == OldTasks.Text) taskId = task.id;
            }
            
            
            string adress = "https://realityleap-rosseti.web.app?type=task&task_id=" + taskId + "&result_id=3";
            Report.NavigateUri= new Uri(adress);
        }

        private void Report_Copy_Click(object sender, RoutedEventArgs e)
        {
            string taskId = "0";
            string damId = "0";
            foreach (var dam in damages)
            {
                if (dam.inspection_task.place.name == Damage.Text)
                {
                    taskId = dam.inspection_task.id;
                    damId = dam.id;
                    break;
                }
            }
            string adress = "https://realityleap-rosseti.web.app?type=result"  + "&result_id=" + damId;
            Report_Copy.NavigateUri = new Uri(adress);
        }


        public async void loadDamage()
        {
            var firebase = new FirebaseClient("https://test-8265d.firebaseio.com/");
            var bd = firebase.Child("inspection_results");
            await bd.PutAsync(damages);
        }
        private void ChekBug_Click(object sender, RoutedEventArgs e)
        {
            //debuger.Text = Damage.Text;
            if (Damage.Text.Length > 2)
            {
                foreach (var report in damages)
                {
                   // debuger.Text = Damage.Text+" "+ dam.inspection_task.place.name;
                    if (report.inspection_task.place.name == Damage.Text)
                    {
                        foreach (var dam in report.defects)
                        {
                            if (dam.location == DamageList.Text)
                            {
                                dam.check = "yes";
                                DamageList.Items.Remove(dam.location);
                                if (DamageList.Items.Count <= 0) {
                                    int dateUtc = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                                    report.approve_time = dateUtc;
                                    loadDamage();
                                }
                            }
                        }

                    }
                }
                loadDamage();

            }
        }

        private void Damage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DamageList.Items.Clear();
            foreach (var report in damages)
            {
                
                if (report.inspection_task.place.name == Damage.Text)
                {
                    foreach (var dam in report.defects)
                    {
                        if (dam.check!="yes") DamageList.Items.Add(dam.location);
                    }
                    break;
                }
            }
        }

        private void DeleteBug_Click(object sender, RoutedEventArgs e)
        {
            if (Damage.Text.Length > 2)
            {
                foreach (var report in damages)
                {
                    // debuger.Text = Damage.Text+" "+ dam.inspection_task.place.name;
                    if (report.inspection_task.place.name == Damage.Text)
                    {
                        foreach (var dam in report.defects)
                        {
                            if (dam.location == DamageList.Text)
                            {
                                DamageList.Items.Remove(dam.location);
                                if (DamageList.Items.Count <= 0)
                                {
                                    int dateUtc = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                                    report.approve_time = dateUtc;
                                    loadDamage();
                                    
                                }
                                report.defects.Remove(dam);
                                break;

                            }
                        }

                    }
                }
                loadDamage();

            }
        }
    }
}
