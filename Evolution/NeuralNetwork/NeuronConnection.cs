using System;

namespace NeuralNetwork
{
    internal class NeuronConnection
    {
        private Neuron Input;
        private Neuron Output;
        private double Weight;

        public NeuronConnection(Neuron input, Neuron output, double weight)
        {
            if (weight < -2.0 || weight > 2.0)
            {
                throw new ArgumentException("Weight should be between -1.0 and +1.0!");
            }

            this.Input = input ?? throw new ArgumentNullException(nameof(input));
            this.Output = output ?? throw new ArgumentNullException(nameof(output));
            this.Weight = weight;
        }

        public double Value
        {
            get
            {
                return this.Weight * this.Input.Value;
            }
        }
    }
}
