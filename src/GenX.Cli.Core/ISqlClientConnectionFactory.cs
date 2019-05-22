namespace GenX.Cli.Core
{
    public interface ISqlClientConnectionFactory
    {
        ISqlClientConnection Create(string connectionString);
    }
}
