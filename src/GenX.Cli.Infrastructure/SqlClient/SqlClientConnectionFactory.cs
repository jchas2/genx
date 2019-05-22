using GenX.Cli.Core;

namespace GenX.Cli.Infrastructure.SqlClient
{
    public class SqlClientConnectionFactory : ISqlClientConnectionFactory
    {
        public ISqlClientConnection Create(string connectionString)
        {
            return new SqlClientConnection(connectionString);
        }
    }
}
