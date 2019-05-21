using System;

namespace GenX.Cli.Core
{
    public static class ObjectExtensions
    {
        public static bool IsNull<T>(this T obj) where T : class
        {
            return obj == null;
        }
    }
}
