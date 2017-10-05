using System.Collections.Generic;

using Genetics;

namespace Evolution.Models
{
    public class BlobSkeleton
    {
        public List<int> BrainLayerSizes { get; set; } = new List<int>();
        public List<double> Genes { get; set; } = new List<double>();

        public BlobSkeleton()
        {

        }

        public BlobSkeleton(List<int> brainLayerSizes, Genome genome)
        {
            this.BrainLayerSizes = brainLayerSizes;
            this.Genes = genome.Genes;
        }
    }
}
