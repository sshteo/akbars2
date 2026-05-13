using System;
using akbars.Data;
using akbars.Models;
using akbars.Repositories;

namespace akbars.Services
{
    public class AuthService : IAuthService
    {
        private readonly Database _database;
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;

        public AuthService(Database database, IUserRepository userRepository, PasswordHasher passwordHasher)
        {
            _database = database;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public bool CanConnect(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!_database.HasConfiguredConnectionString())
            {
                errorMessage = "Не настроена строка подключения PostgresConnection в App.config.";
                return false;
            }

            try
            {
                using (var connection = _database.GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Не удалось подключиться к базе данных. " + ex.Message;
                return false;
            }
        }

        public AuthResult Authenticate(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                return new AuthResult { ErrorMessage = "Введите логин и пароль." };
            }

            try
            {
                var user = _userRepository.GetByLogin(login.Trim());
                if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
                {
                    return new AuthResult { ErrorMessage = "Неверный логин или пароль." };
                }

                if (!string.IsNullOrWhiteSpace(user.PasswordHash) &&
                    !user.PasswordHash.StartsWith("pbkdf2$", StringComparison.OrdinalIgnoreCase))
                {
                    _userRepository.UpdatePasswordHash(user.Id, _passwordHasher.Hash(password));
                }

                return new AuthResult
                {
                    Success = true,
                    Session = new SessionContext
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        MiddleName = user.MiddleName,
                        Email = user.Email,
                        Phone = user.Phone,
                        Department = user.Department,
                        RoleId = user.RoleId,
                        RoleName = user.RoleName
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    ErrorMessage = "Ошибка входа: " + ex.Message
                };
            }
        }
    }
