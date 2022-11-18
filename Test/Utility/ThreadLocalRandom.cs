using System;
using System.Threading;

namespace Test.Utility;

internal static class ThreadLocalRandom
{
    private static readonly Random _global = new Random();
    [ThreadStatic]
    private static Random? _local;

    public static Random Instance
    {
        get
        {
            if (_local is null)
            {
                int seed;
                lock (_global)
                {
                    seed = _global.Next();
                }
                _local = new Random(seed);
            }

            return _local;
        }
    }
}
