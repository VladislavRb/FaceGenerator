using System;
using System.Linq;

namespace FaceGenerator.MachineLearning.Helpers
{
    public static class NullCheckHelper
    {
        public static void ThrowIfNull(object arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException($"Argument '{nameof(arg)}' can't be null");
            }
        }

        public static void ThrowIfAllNull(params object[] args)
        {
            if (args.All(arg => arg == null))
            {
                throw new ArgumentNullException(nameof(args), "All arguments are null");
            }
        }
    }
}
