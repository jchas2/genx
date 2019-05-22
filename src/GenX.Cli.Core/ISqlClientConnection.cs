using System;
using System.Data;

namespace GenX.Cli.Core
{
    public interface ISqlClientConnection : IDisposable
    {
        void Open();
        IDbCommand CreateCommand();
        DataTable GetSchema(string collectionName);
        DataTable GetSchema(string collectionName, string[] restrictionValues);
    }
}
