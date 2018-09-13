using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class NeuralNetwork
    {
        public List<Layer> layers;
        readonly double learningRate;


        public NeuralNetwork(double learningRate, int[] layers)
        {
            if (layers.Length < 2) throw new System.ArgumentException("La red debe contener al menos 3 capas");

            this.learningRate = learningRate;
            this.layers = new List<Layer>();

            //primera layer
            Layer primera = new Layer(layers[0]);
            this.layers.Add(primera);

            // primera hidden layer
            Layer primeraHidden = new Layer(layers[1]);
            this.layers.Add(new Layer(layers[1], primera));

            //hidden layers last
            for (int i = 2; i < layers.Length; i++)
            {
                this.layers.Add(new Layer(layers[i], this.layers[i - 1]));
            }


        }


                /**
         * Train a List of DataSet 
         * */
        public void Train(List<DataSet> sets, int epocas)
        {
            for (int i = 0; i < epocas; i++)
            {
                foreach (DataSet ds in sets)
                {
                    Feedforward(ds);
                    Backpropagation(ds);
                }
            }
        }

        /**
         * Train a single DataSet
         * */
        public void Train(DataSet set, int muestra, int epoca)
        {
            Feedforward(set);
            Backpropagation(set);
        }


        private void Feedforward(DataSet ds)
        {
            if (ds.inputs.Length != layers[0].neuronas.Count)
                throw new System.Exception("La cantidad de inputs en el DataSet difiere con la " +
                    "cant de neuronas en la primer capa");

            int i = 0;
            layers[0].neuronas.ForEach(n => n.a = ds.inputs[i++]);
            layers.Skip<Layer>(1).ToList<Layer>().ForEach(l => l.Activacion());
        }


        private void Backpropagation(DataSet ds)
        {
            if (ds.outputs.Length != layers[layers.Count - 1].neuronas.Count)
                throw new System.Exception("La cantidad de outputs en el DataSet difiere con la " +
                    "cant de neuronas en la ultima capa");

            //calculo delta en la ultima capa. 
            int j = 0;
            layers[layers.Count - 1].neuronas.ForEach(n => n.Delta(ds.outputs[j++]));


            // calculo delta en las capas intermedias hacia atras
            for (int i = layers.Count - 2; i >= 1; i--)
            {
                layers[i].neuronas.ForEach(n => n.Delta());
            }

            // update todos los weights y bias
            for (int i = layers.Count - 1; i >= 1; i--)
            {
                layers[i].neuronas.ForEach(n => n.Update(learningRate));
            }

            //string ret = layers[layers.Count - 1].neuronas.Sum(n => Math.Pow(n.error, 2)).ToString("0.00000") + "\n";

            //int k = 0;
            //foreach (Neuron n in layers[layers.Count - 1].neuronas)
            //{
            //    // er= a -y
            //    ret += (k++).ToString() + ": " + n.error.ToString("0.00000") + " = " + n.a.ToString("0.00000") + " - " + n.y.ToString("0.00000") + "\n";
            //}
            //return ret;
        }


        public List<Neuron> Test(DataSet ds)
        {
            Feedforward(ds);
            return layers[layers.Count - 1].neuronas;
        }
    }


    public class DataSet
    {
        public double[] inputs;
        public double[] outputs;

        public DataSet(double[] inputs, double[] outputs)
        {
            this.inputs = inputs;
            this.outputs = outputs;
        }

    }


    public class Dentrite
    {
        public Neuron inputNeuron;
        public Neuron outputNeuron;
        public double weight;
        static Random random = new Random();

        public Dentrite(Neuron input, Neuron output)
        {
            weight = random.NextDouble() * (0.5 - (-0.5)) + (-0.5);
            inputNeuron = input;
            outputNeuron = output;
        }
    }


    public class Formulas
    {
        public static double Sigmoid(double x)
        {
            //return x< -45.0 ? 0.0 : x > 45.0 ? 1.0 : 1.0 / (1.0 + Math.Exp(-x));
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        public static double SigmoidDx(double x)
        {
            return x * (1 - x);
        }
    }



    public class Layer
    {
        public List<Neuron> neuronas;

        //para la primera capa
        public Layer(int cantNeuronas)
        {
            neuronas = new List<Neuron>();

            for (int i = 0; i < cantNeuronas; i++)
            {
                neuronas.Add(new Neuron());
            }
        }

        //para las capas que no son la primera
        public Layer(int cantNeuronas, Layer anterior)
        {
            neuronas = new List<Neuron>();

            for (int i = 0; i < cantNeuronas; i++)
            {
                neuronas.Add(new Neuron(anterior));
            }
        }

        public void Activacion()
        {
            neuronas.ForEach(n => n.Activacion());
        }
    }


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
            bias = random.NextDouble() * (0.5 - (-0.5)) + (-0.5);

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
            delta = outputDentrites.Sum(d => d.weight * d.outputNeuron.delta) * Formulas.SigmoidDx(a);
        }

        public void Update(double learningRate)
        {
            bias -= learningRate * delta;
            dentritas.ForEach(d => d.weight -= learningRate * delta * d.inputNeuron.a);
        }

        public override string ToString()
        {
            return "Neurona [a=" + a.ToString("0.0000000000") + " delta=" + delta + " err=" + error + "]";
        }
    }



}
