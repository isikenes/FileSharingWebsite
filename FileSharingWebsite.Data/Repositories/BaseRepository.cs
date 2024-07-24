using Dapper;
using FileSharingWebsite.Data.Context;
using System.Data;

namespace FileSharingWebsite.Data.Repositories
{
    public abstract class BaseRepository
    {
        private readonly DapperDBContext context;

        protected BaseRepository(DapperDBContext context)
        {
            this.context = context;
        }

        protected void Execute(string query, object parameters = null)
        {
            using (IDbConnection db = context.CreateConnection())
            {
                db.Execute(query, parameters);
            }

        }

        protected T? Query<T>(string query, object parameters = null)
        {
            using (IDbConnection db = context.CreateConnection())
            {
                return db.QueryFirstOrDefault<T>(query, parameters);
            }
        }

        protected IEnumerable<T>? ListQuery<T>(string query, object parameters = null)
        {
            using (IDbConnection db = context.CreateConnection())
            {
                return db.Query<T>(query, parameters).ToList();
            }
        }

    }
}
