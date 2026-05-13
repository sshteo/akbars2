using System.Linq;
using System.Windows;
using akbars.Services;

namespace akbars.Views.Dispatcher
{
    public partial class WorkersDispetcher : Window
    {
        public WorkersDispetcher()
        {
            InitializeComponent();
            EmployeesGrid.ItemsSource = AppServices.UserService.GetUsers(2).OrderBy(user => user.LastName).ToList();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
