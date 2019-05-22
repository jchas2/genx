using GenX.Cli.Core;
using System.Data;
using System.Data.SqlClient;

namespace GenX.Cli.Infrastructure.SqlClient
{
    public class SqlClientConnection : ISqlClientConnection
    {
        private SqlConnection _sqlConnection;
        private readonly string _connectionString;

        public SqlClientConnection(string connectionString) => _connectionString = connectionString;

        ~SqlClientConnection()
        {
            Dispose();
        }

        public void Open()
        {
            Dispose();
            _sqlConnection = new SqlConnection(_connectionString);
            _sqlConnection.Open();
        }

        public IDbCommand CreateCommand()
        {
            return _sqlConnection.CreateCommand();
        }

        public DataTable GetSchema(string collectionName)
        {
            return _sqlConnection.GetSchema(collectionName);
        }

        public DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return _sqlConnection.GetSchema(collectionName, restrictionValues);
        }

        public void Dispose()
        {
            if (_sqlConnection != null)
            {
                _sqlConnection.Dispose();
            }
        }
    }
}
