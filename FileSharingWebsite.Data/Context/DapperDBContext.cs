using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace FileSharingWebsite.Data.Context
{
    public class DapperDBContext
    {
        private readonly IConfiguration configuration;

        private readonly string? connectionString;

        public DapperDBContext(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration.GetConnectionString("connection");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
