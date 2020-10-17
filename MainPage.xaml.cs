﻿using Firebase.Database;
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
        List<Task> tasks;
        public MainPage()
        {


            this.InitializeComponent();
            DBload();
        }
        public async void DBload()
        {
            var firebase = new FirebaseClient("https://realityleap-rosseti.firebaseio.com/");
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
            tasks = await firebase.Child("inspection_tasks")
                .OrderByKey()
                .OnceSingleAsync<List<Task>>();
            showTasks();
        }

        public void showTasks()
        {
            foreach (ItemCollection item in OldTasks.Items) OldTasks.Items.Remove(item);

            foreach (var task in tasks)
            {
                if (($"{ task.creator.middle_name} { task.creator.first_name} { task.creator.last_name}")== MasterEmployer.Text)
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
            Task newTask = new Task { creator = NewMaster, executor = NewEmployers, id = 5, place = newPlace, safety_event = Task.Text };
            var firebase = new FirebaseClient("https://realityleap-rosseti.firebaseio.com/");
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

        private void MasterEmployer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showTasks();
        }

        private void OldTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int taskId=0;
            
            string adress = "https://realityleap-rosseti.web.app?type=task&task_id=" + taskId.ToString() + "&result_id=3";
            Report.NavigateUri= new Uri(adress);
        }
    }
}
