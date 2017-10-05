using Evolution.Core;
using NeuralNetwork.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
    public class Network
    {
        public List<int> LayerSizes { get; }

        private NeuralLayer InputLayer;
        private NeuralLayer OutputLayer;
        private List<NeuralLayer> HiddenLayers = new List<NeuralLayer>();
        private IActivationFunction ActivationFunction;

        public Network(List<int> layerSizes, IActivationFunction activationFunction, RandomHelper randomHelper)
        {
            if (layerSizes == null)
            {
                throw new ArgumentNullException(nameof(layerSizes));
            }

            if (randomHelper == null)
            {
                throw new ArgumentNullException(nameof(randomHelper));
            }

            if (layerSizes.Count < 2)
            {
                throw new ArgumentException($"At least 2 layers are needed (an input and an output layer)");
            }

            this.ActivationFunction = activationFunction ?? throw new ArgumentNullException(nameof(activationFunction));

            var valuesNeeded = CalculateAmountOfNeededValues(layerSizes);
            var layerBiasesAndWeights = Enumerable
                .Range(0, valuesNeeded)
                .Select(i => randomHelper.DoubleBetween(-1, 1))
                .ToList();

            this.InitializeLayers(layerSizes, layerBiasesAndWeights);
            this.LayerSizes = layerSizes;
        }

        public Network(List<int> layerSizes, List<double> layerBiasesAndWeights, IActivationFunction activationFunction)
        {
            if (layerSizes == null)
            {
                throw new ArgumentNullException(nameof(layerSizes));
            }

            if (layerBiasesAndWeights == null)
            {
                throw new ArgumentNullException(nameof(layerBiasesAndWeights));
            }

            if (layerSizes.Count < 2)
            {
                throw new ArgumentException($"At least 2 layers are needed (an input and an output layer)");
            }

            var amountOfNeededValues = CalculateAmountOfNeededValues(layerSizes);
            if (amountOfNeededValues != layerBiasesAndWeights.Count)
            {
                throw new ArgumentException($"{nameof(layerBiasesAndWeights)} should have a length of {amountOfNeededValues}. Here: {layerBiasesAndWeights.Count}");
            }

            this.ActivationFunction = activationFunction ?? throw new ArgumentNullException(nameof(activationFunction));

            this.InitializeLayers(layerSizes, layerBiasesAndWeights);
            this.LayerSizes = layerSizes;
        }

        public List<double> Calculate(List<double> inputValues)
        {
            this.InputLayer.Calculate(inputValues);
            this.HiddenLayers.ForEach(hl => hl.Calculate());
            return this.OutputLayer.Calculate();
        }

        public List<double> Values()
        {
            return this.OutputLayer.Values;
        }

        public static int CalculateAmountOfNeededValues(List<int> layerSizes)
        {
            int length = layerSizes[0];

            for (int i = 1; i < layerSizes.Count; i++)
            {
                length += layerSizes[i - 1] * layerSizes[i] + layerSizes[i];
            }

            return length;
        }

        private void InitializeLayers(List<int> layerSizes, List<double> layerBiasesAndWeights)
        {
            this.AddInputLayer(layerBiasesAndWeights.Take(layerSizes[0]).ToList());

            int currentLayerStartingIndex = 0;
            int currentLayerLength = layerSizes[0];
            for (int i = 1; i < layerSizes.Count - 1; i++)
            {
                currentLayerStartingIndex += currentLayerLength;
                currentLayerLength = layerSizes[i - 1] * layerSizes[i] + layerSizes[i];
                this.AddLayer(layerBiasesAndWeights.Skip(currentLayerStartingIndex).Take(currentLayerLength).ToList(), layerSizes[i - 1], false);
            }

            currentLayerStartingIndex += currentLayerLength;
            this.AddLayer(layerBiasesAndWeights.Skip(currentLayerStartingIndex).ToList(), layerSizes[layerSizes.Count - 2], true);
        }

        private void AddLayer(List<double> values, int weightsCount, bool isOutputLayer)
        {
            var weights = new List<List<double>>();
            var biases = new List<double>();
            for (int i = 0; i < values.Count; i += weightsCount + 1)
            {
                biases.Add(values[i]);
                weights.Add(values.Skip(i + 1).Take(weightsCount).ToList());
            }

            if (isOutputLayer)
            {
                this.AddOutputLayer(weights, biases);
            }
            else
            {
                this.AddHiddenLayer(weights, biases);
            }
        }

        private void AddInputLayer(List<double> biases)
        {
            this.InputLayer = new NeuralLayer(biases, this.ActivationFunction);
        }

        private void AddOutputLayer(List<List<double>> weights, List<double> biases)
        {
            this.OutputLayer = new NeuralLayer(this.HiddenLayers.LastOrDefault() ?? this.InputLayer, weights, biases, this.ActivationFunction);
        }

        private void AddHiddenLayer(List<List<double>> weights, List<double> biases)
        {
            this.HiddenLayers.Add(new NeuralLayer(this.HiddenLayers.LastOrDefault() ?? this.InputLayer, weights, biases, this.ActivationFunction));
        }
    }
}
