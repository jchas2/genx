using GenX.Cli.Core.Commands.Generate;

namespace GenX.Cli.Core
{
    public interface ITransformer
    {
        void Transform(Configuration configuration);
    }
}
