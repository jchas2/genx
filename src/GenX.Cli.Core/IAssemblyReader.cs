namespace GenX.Cli.Core
{
    public interface IAssemblyReader
    {
        AssemblyModel Read(string filename, string namespaceFilter);
    }
}
