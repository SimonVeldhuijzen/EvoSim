using System;

namespace NeuralNetwork.ActivationFunctions
{
    public class Sigmoid : IActivationFunction
    {
        public double Execute(double x)
        {
            return 2.0 / (1.0 + Math.Exp(-x)) - 1.0;
        }
    }
}
