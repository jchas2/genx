using System.Drawing;

namespace GenX.Cli.Tests.TestAssembly
{
    public class PublicGenericClassT<T> 
    {
        private readonly T _variable;

        public PublicGenericClassT(T variable)
        {
            _variable = variable;
        }

        private static T PrivateStaticVariable { get; set; }

        public T PublicInstanceVariable { get; set; }

        public void PublicInstanceDoNothing() { }

        private void PrivateInstanceDoNothing() { }

        public Point PublicInstanceCalculatePoint(int x, int y)
        {
            return new Point(x, y);
        }
    }
}
