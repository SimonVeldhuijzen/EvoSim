using NeuralNetwork.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
    internal class NeuralLayer
    {
        private List<Neuron> Neurons;

        public NeuralLayer(NeuralLayer inputLayer, List<List<double>> weights, List<double> biases, IActivationFunction activationFunction)
        {
            if (weights.Count != biases.Count)
            {
                throw new ArgumentException($"Each List in {nameof(weights)} should have as many entries as there are {nameof(biases)}");
            }

            if (weights.Any(w => w.Count != inputLayer.Neurons.Count))
            {
                throw new ArgumentException($"Each List in {nameof(weights)} should have as many entries as there are neurons in the {nameof(inputLayer)}");
            }

            this.Neurons = new List<Neuron>();

            for (int i = 0; i < biases.Count; i++)
            {
                var weightsForNeuron = new Dictionary<Neuron, double>();
                for (int j = 0; j < inputLayer.Neurons.Count; j++)
                {
                    weightsForNeuron.Add(inputLayer.Neurons[j], weights[i][j] * 2);
                }

                this.Neurons.Add(new Neuron(weightsForNeuron, biases[i], activationFunction));
            }
        }

        public NeuralLayer(List<double> biases, IActivationFunction activationFunction)
        {
            this.Neurons = biases.Select(b => new Neuron(b, activationFunction)).ToList();
        }

        public List<double> Calculate()
        {
            return this.Neurons.Select(n => n.Calculate()).ToList();
        }

        public void Calculate(List<double> inputValues)
        {
            if (inputValues.Count != this.Neurons.Count)
            {
                throw new ArgumentException($"Count of {nameof(inputValues)} should equal count of {nameof(this.Neurons)}");
            }

            for (int i = 0; i < inputValues.Count; i++)
            {
                this.Neurons[i].SetValue(inputValues[i]);
            }
        }

        public List<double> Values
        {
            get
            {
                return this.Neurons.Select(n => n.Value).ToList();
            }
        }
    }
}
