using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rosseti
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        List<Employer> employers;
        public BlankPage1()
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
                
                if (empl.role == "мастер") Master.Items.Add($"{ empl.first_name} { empl.middle_name} { empl.last_name}");
            }
            
        }

        private void logining_Click(object sender, RoutedEventArgs e)
        {
            if (Master.Text.Length>2&&password.Text.Length>2)  this.Frame.Navigate(typeof(MainPage),Master.Text);
        }

        private void MasterEmployer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
