namespace GenX.Cli.Core
{
    public interface IDbSchemaReader
    {
        DbModel Read(string connectionString);        
    }
}
