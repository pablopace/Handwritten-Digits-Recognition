using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class Neuron
    {
        public double bias;
        public double a; //activacion 
        public List<Dentrite> dentritas;
        List<Dentrite> outputDentrites;
        public double delta;
        public double error;
        static Random random = new Random();
        public double y;

        public Neuron()
        {
            outputDentrites = new List<Dentrite>();

        }

        public Neuron(Layer anterior) : this()
        {
            dentritas = new List<Dentrite>();
            bias = random.NextDouble() * (1 - (-1)) + (-1);

            foreach (Neuron n in anterior.neuronas)
            {
                Dentrite dentrita = new Dentrite(n, this);
                dentritas.Add(dentrita);
                n.outputDentrites.Add(dentrita);
            }
        }

        public void Activacion()
        {
            a = Formulas.Sigmoid(bias + dentritas.Sum(d => d.weight * d.inputNeuron.a));

        }

        //delta para neuronas de ultima capa (con dataset.outputs)
        public void Delta(double y)
        {
            this.y = y;
            error = a - y;
            delta = 2 * error * Formulas.SigmoidDx(a);
        }

        public void Delta()
        {
            delta = outputDentrites.Sum(d => d.weight * d.outputNeuron.delta)* Formulas.SigmoidDx(a);
        }

        public void Update(double learningRate)
        {
            bias -= learningRate * delta;
            dentritas.ForEach(d => d.weight -= learningRate * delta * d.inputNeuron.a);
        }

        public override string ToString()
        {
            return "Neurona [a="+a.ToString("0.0000000000") +" delta="+delta+" err="+error+"]";
        }
    }
}
