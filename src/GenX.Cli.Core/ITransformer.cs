using GenX.Cli.Core.Commands.Generate;

namespace GenX.Cli.Core
{
    public interface ITransformer
    {
        string Transform(Configuration configuration);
    }
}
