using System;
using System.Drawing;

namespace Evolution.Core
{
    public class RandomHelper
    {
        private Random Random = new Random();

        public double DoubleBetween(double min, double max)
        {
            if (min >= max)
            {
                throw new ArgumentException($"{nameof(min)} should be lower than {nameof(max)}");
            }

            var difference = max - min;
            var tuning = max - difference;

            return this.Random.NextDouble() * difference + tuning;
        }

        public bool WeightedCoinThrow(double chanceForHeads)
        {
            if (chanceForHeads < 0 || chanceForHeads > 1)
            {
                throw new ArgumentException($"{nameof(chanceForHeads)} should be between 0 and 1. Here: {chanceForHeads}");
            }

            return this.Random.NextDouble() < chanceForHeads;
        }

        public int Next(int min, int max)
        {
            if (min >= max)
            {
                throw new ArgumentException($"{nameof(min)} should be lower than {nameof(max)}");
            }

            return this.Random.Next(min, max);
        }

        public Point GeneratePointWithin(Size size)
        {
            return new Point(this.Random.Next(size.Width), this.Random.Next(size.Height));
        }
    }
}
