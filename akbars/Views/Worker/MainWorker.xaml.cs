using akbars.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace akbars.Views.Worker
{
    public partial class MainWorker : Window
    {
        public MainWorker(
            string lastName,
            string firstName,
            string middleName,
            string email,
            string phone,
            string department,
            string role)
        {
            InitializeComponent();

            HelloText.Text = $"Привет, {lastName} {firstName}!";

            LastNameText.Text = lastName;
            FirstNameText.Text = firstName;
            MiddleNameText.Text = middleName;

            EmailText.Text = email;
            PhoneText.Text = phone;

            DepartmentText.Text = department;
            RoleText.Text = role;
        }



        private void Tickets_Click(object sender, RoutedEventArgs e)
        {
            // Получи userId откуда-то (из настроек/поля/сессии)
            int currentUserId = 1; // или Properties.Settings.Default.UserId

            TicketsWorker ticketsWindow = new TicketsWorker(currentUserId);
            ticketsWindow.Show();
         //   this.Close();
        }




        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Main mainWindow = new Main();
            mainWindow.Show();
            this.Close();
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}