using FileSharingWebsite.Data.Repositories;
using FileSharingWebsite.Entities.Models;

namespace FileSharingWebsite.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public void ChangeEmail(int id, string email)
        {
            userRepository.ChangeEmail(id, email);
        }

        public void CreateUser(User user)
        {
            userRepository.CreateUser(user);
        }

        public void DeleteUser(int id)
        {
            userRepository.DeleteUser(id);
        }

        public string GetEmail(int id)
        {
            return userRepository.GetEmail(id);
        }

        public string GetPassword(int id)
        {
            return userRepository.GetPassword(id);
        }

        public User GetUser(int id)
        {
            return userRepository.GetUser(id);
        }

        public User GetUserByEmail(string email)
        {
            return userRepository.GetUserByEmail(email);
        }

        public void SetPassword(int id, string password)
        {
            userRepository.SetPassword(id, password);
        }

        public void UpdateUser(User user)
        {
            userRepository.UpdateUser(user);
        }
    }
}
