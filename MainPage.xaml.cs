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
    public sealed partial class MainPage : Page
    {
        List<Employer> employers;
        List<Places> places;
        public MainPage()
        {


            this.InitializeComponent();
            DBload();
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
                if (empl.role == "мастер") MasterEmployer.Items.Add($"{ empl.middle_name} { empl.first_name} { empl.last_name}");
            }
            places = await firebase.Child("places")
                .OrderByKey()
                .OnceSingleAsync<List<Places>>();

            foreach (var place in places)
            {
                PlaceWorkChoose.Items.Add($"{ place.name}");
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
        

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if ((PlaceWorkChoose.Text.Length>2)&& (Task.Text.Length > 10 )&& (EmployerChoose.Text.Length > 2) && (MasterEmployer.Text.Length > 2))
            {
                debuger.Text = PlaceWorkChoose.Text.Length+" "+Task.Text.Length+ " " +EmployerChoose.Text.Length+" " +MasterEmployer.Text.Length;
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
            Task newTask = new Task { creator = NewMaster, executor = NewEmployers, id = 5, place = newPlace, safety_event = Task.Text };
            var firebase = new FirebaseClient("https://test-8265d.firebaseio.com/");
            var bd = firebase.Child("inspection_tasks");
            var tasksArray = await firebase.Child("inspection_tasks")
                .OrderByKey()
                .OnceSingleAsync<List<Task>>();
            newTask.id = tasksArray.Count;
            tasksArray.Add(newTask);

            await bd.PutAsync(tasksArray);

        }

        private void Task_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
