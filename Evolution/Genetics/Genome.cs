using Evolution.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genetics
{
    public class Genome
    {
        public List<double> Genes { get; set; }

        private RandomHelper RandomHelper;

        public Genome(RandomHelper randomHelper, int length)
        {
            if (length < 1)
            {
                throw new ArgumentException($"{nameof(length)} should be at least 1. Here: {length}");
            }

            this.RandomHelper = randomHelper ?? throw new ArgumentNullException(nameof(randomHelper));

            this.Genes = Enumerable
                .Range(0, length)
                .Select(i => RandomHelper.DoubleBetween(-1, 1))
                .ToList();
        }

        public Genome(RandomHelper randomHelper, IReadOnlyList<double> genes)
        {
            if (genes.Count < 1)
            {
                throw new ArgumentException($"Count of {nameof(genes)} should be at least 1. Here: {genes.Count}");
            }

            this.RandomHelper = randomHelper ?? throw new ArgumentNullException(nameof(randomHelper));

            this.Genes = new List<double>(genes);
        }

        public void Mutate(double chance)
        {
            if (chance < 0 || chance > 1)
            {
                throw new ArgumentException($"{nameof(chance)} should be between 0 and 1. Here: {chance}");
            }

            for (int i = 0; i < this.Genes.Count; i++)
            {
                if (RandomHelper.WeightedCoinThrow(chance))
                {
                    this.Genes[i] = this.MutateDouble(this.Genes[i]);
                }
            }
        }

        public Genome Clone(double mutateChance = 0.1)
        {
            if (mutateChance < 0.0 || mutateChance > 1.0)
            {
                throw new ArgumentException($"{nameof(mutateChance)} should be between 0.0 and 1.0. Here: {mutateChance}");
            }

            var newGenome = new Genome(this.RandomHelper, this.Genes);
            newGenome.Mutate(mutateChance);
            return newGenome;
        }

        public Tuple<Genome, Genome> PairWith(Genome other, double mutateChance = 0.05)
        {
            if (mutateChance < 0.0 || mutateChance > 1.0)
            {
                throw new ArgumentException($"{nameof(mutateChance)} should be between 0.0 and 1.0. Here: {mutateChance}");
            }

            if (this.Genes.Count != other.Genes.Count)
            {
                throw new ArgumentException($"Genomes are not compatible: this one has {this.Genes.Count} {nameof(this.Genes)}, the other has {other.Genes.Count}");
            }

            var cutoffIndex = RandomHelper.Next(0, this.Genes.Count);
            var genesOfFirstGenome = this.Genes.Take(cutoffIndex).Concat(other.Genes.Skip(cutoffIndex));
            var genesOfSecondGenome = other.Genes.Take(cutoffIndex).Concat(this.Genes.Skip(cutoffIndex));

            var firstGenome = new Genome(this.RandomHelper, genesOfFirstGenome.ToList());
            var secondGenome = new Genome(this.RandomHelper, genesOfSecondGenome.ToList());

            firstGenome.Mutate(mutateChance);
            secondGenome.Mutate(mutateChance);

            return Tuple.Create(firstGenome, secondGenome);
        }

        private double MutateDouble(double original)
        {
            if (original < -1.0 || original > 1.0)
            {
                throw new ArgumentException($"{nameof(original)} should be between -1.0 and +1.0. Here: {original}");
            }

            var result = original + RandomHelper.DoubleBetween(-0.5, 0.5);
            if (result > 1.0)
            {
                result = 1.0;
            }
            else if (result < -1.0)
            {
                result = -1.0;
            }

            return result;
        }
    }
}
