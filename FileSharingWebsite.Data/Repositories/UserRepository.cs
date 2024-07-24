using FileSharingWebsite.Data.Context;
using FileSharingWebsite.Entities.Models;

namespace FileSharingWebsite.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(DapperDBContext context) : base(context)
        {
        }

        public void ChangeEmail(int id, string email)
        {
            string query = @"update dbo.Users set email=@Email where user_id=@Id";
            Execute(query, new { Email = email, Id = id });
        }

        public void CreateUser(User user)
        {
            string query = @"insert into dbo.Users (email, password) values (@Email, @Password)";
            Execute(query, new { user.Email, user.Password });
        }

        public User GetUser(int id)
        {
            string query = @"select user_id as UserId, email, password from dbo.Users where user_id=@Id";
            return Query<User>(query, new { Id = id });
        }

        public User GetUserByEmail(string email)
        {
            string query = @"select user_id as UserId, email, password from dbo.Users where email=@Email";
            return Query<User>(query, new { Email = email });
        }

        public string GetEmail(int id)
        {
            string query = @"select email from dbo.Users where user_id=@Id";
            return Query<string>(query, new { Id = id });
        }

        public string GetPassword(int id)
        {
            string query = @"select password from dbo.Users where user_id=@Id";
            return Query<string>(query, new { Id = id });
        }

        public void SetPassword(int id, string password)
        {
            string query = @"update dbo.Users set password=@Password where user_id=@Id";
            Execute(query, new { Password = password, Id = id });
        }

        public void DeleteUser(int id)
        {
            string query = @"delete from dbo.Users where user_id=@Id";
            Execute(query, new { Id = id });
        }

        public void UpdateUser(User user)
        {
            string query = @"update dbo.Users set email=@Email, password=@Password where user_id=@UserId";
            Execute(query, new { Email = user.Email, Password = user.Password, UserId = user.UserId });
        }
    }
}
