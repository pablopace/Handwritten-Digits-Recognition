using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class Dentrite
    {
        public Neuron inputNeuron;
        public Neuron outputNeuron;
        public double weight;
        static Random random = new Random();

        public Dentrite(Neuron input, Neuron output)
        {
            weight = random.NextDouble() * (0.3 - (-0.3)) + (-0.3);
            inputNeuron = input;
            outputNeuron = output;
        }
    }

   
}
