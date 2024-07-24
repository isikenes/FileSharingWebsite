using FileSharingWebsite.Entities.Models;

namespace FileSharingWebsite.Data.Repositories
{
    public interface IUserRepository
    {
        void CreateUser(User user);
        User GetUser(int id);
        User GetUserByEmail(string email);
        void ChangeEmail(int id, string email);
        string GetEmail(int id);
        string GetPassword(int id);
        void SetPassword(int id, string password);
        void DeleteUser(int id);
        void UpdateUser(User user);
    }
}
