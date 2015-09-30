using System;
using System.Threading;

namespace WeChatUtil
{
    public class RandomGenerator
    {
        private static int _seed = Environment.TickCount;
        private readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        private static readonly RandomGenerator s_randomGenerator = new RandomGenerator();

        public static RandomGenerator Instance { get { return s_randomGenerator; } }

        private RandomGenerator()
        {
        }

        public int Next()
        {
            return _random.Value.Next();
        }
        public int Next(int max)
        {
            return _random.Value.Next(max);
        }
        public int Next(int min, int max)
        {
            return _random.Value.Next(min, max);
        }
        public void NextBytes(byte[] buffer)
        {
            _random.Value.NextBytes(buffer);
        }
        public double NextDouble()
        {
            return _random.Value.NextDouble();
        }
    }
}
