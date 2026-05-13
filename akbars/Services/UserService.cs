using System.Collections.Generic;
using akbars.Models;
using akbars.Repositories;

namespace akbars.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User GetUser(int userId)
        {
            return _userRepository.GetById(userId);
        }

        public void UpdateProfile(User user)
        {
            _userRepository.UpdateProfile(user);
        }

        public List<User> GetUsers(int? roleId)
        {
            return _userRepository.GetUsers(roleId);
        }

        public void UpdateRole(int userId, int roleId)
        {
            _userRepository.UpdateRole(userId, roleId);
        }
    }
