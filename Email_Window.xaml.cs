using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace mailSender
{
    /// <summary>
    /// Логика взаимодействия для Email_Window.xaml
    /// </summary>
    public partial class Email_Window : Window
    {
        int idTabNumber;
        string tabNumber;
        string email;
        public Email_Window()
        {
            InitializeComponent();
            DataBaseContext db = new();
            ListSomeText.ItemsSource = db.DataBases.ToList();

        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {

            if (tabelNumber.Text == "")
            {
                downTray.Content = "Не ввели текст!";
                downTray.Background = Brushes.LightCoral;
            }
            else
            {

                DataBaseContext db = new();
                DataBase tim = new DataBase
                {
                    SomeText = tabelNumber.Text,
                    Date = emailOfUser.Text
                };
                db.DataBases.Add(tim);
                db.SaveChanges();

                downTray.Content = "Добавлена запись!";
                downTray.Background = Brushes.LightGreen;
                ListSomeText.ItemsSource = db.DataBases.ToList();
            }
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {

        }
        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataBase path = ListSomeText.SelectedItem as DataBase;
            if (path != null)
            {
                idTabNumber = path.Id;
                tabNumber = Convert.ToString(path.SomeText);
                email = Convert.ToString(path.Date);
            }
           
        }
    }
}
