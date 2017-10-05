using NeuralNetwork.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
    internal class Neuron
    {
        public double Value { get; private set; }

        private double Bias;
        private IActivationFunction ActivationFunction;
        private List<NeuronConnection> InputConnections = new List<NeuronConnection>();
        private List<NeuronConnection> OutputConnections = new List<NeuronConnection>();

        public Neuron(IDictionary<Neuron, double> weights, double bias, IActivationFunction activationFunction)
            :this(bias, activationFunction)
        {
            foreach (var weight in weights)
            {
                this.CreateNeuronConnection(weight.Key, weight.Value);
            }
        }

        public Neuron(double bias, IActivationFunction activationFunction)
        {
            this.Bias = bias;
            this.ActivationFunction = activationFunction ?? throw new ArgumentNullException(nameof(activationFunction));
        }

        public double Calculate()
        {
            this.Value = this.ActivationFunction.Execute(this.InputConnections.Sum(ic => ic.Value) + this.Bias);
            return this.Value;
        }

        public void SetValue(double value)
        {
            if (value < -1.0 || value > 1.0)
            {
                throw new ArgumentException("{nameof(value)} should be between -1.0 and +1.0");
            }

            this.Value = value;
        }

        private void CreateNeuronConnection(Neuron other, double weight)
        {
            var connection = new NeuronConnection(other, this, weight);
            this.InputConnections.Add(connection);
            other.OutputConnections.Add(connection);
        }
    }
}
