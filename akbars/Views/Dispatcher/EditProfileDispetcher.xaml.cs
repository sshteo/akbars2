using System;
using System.Windows;
using akbars.Models;
using akbars.Services;

namespace akbars.Views.Dispatcher
{
    public partial class EditProfileDispetcher : Window
    {
        private readonly SessionContext _session;

        public EditProfileDispetcher(SessionContext session)
        {
            InitializeComponent();
            _session = session;
            LoadUser();
        }

        private void LoadUser()
        {
            var user = AppServices.UserService.GetUser(_session.UserId);
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
                return;
            }

            LastNameBox.Text = user.LastName;
            FirstNameBox.Text = user.FirstName;
            MiddleNameBox.Text = user.MiddleName;
            DepartmentBox.Text = user.Department;
            EmailBox.Text = user.Email;
            PhoneBox.Text = user.Phone;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = new User
                {
                    Id = _session.UserId,
                    LastName = LastNameBox.Text.Trim(),
                    FirstName = FirstNameBox.Text.Trim(),
                    MiddleName = MiddleNameBox.Text.Trim(),
                    Department = DepartmentBox.Text.Trim(),
                    Email = EmailBox.Text.Trim(),
                    Phone = PhoneBox.Text.Trim()
                };

                AppServices.UserService.UpdateProfile(user);
                _session.LastName = user.LastName;
                _session.FirstName = user.FirstName;
                _session.MiddleName = user.MiddleName;
                _session.Department = user.Department;
                _session.Email = user.Email;
                _session.Phone = user.Phone;
                _session.FullName = string.Format("{0} {1} {2}", user.LastName, user.FirstName, user.MiddleName).Trim();

                MessageBox.Show("Профиль обновлён.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось сохранить профиль: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
