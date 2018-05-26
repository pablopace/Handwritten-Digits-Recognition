using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public static class Formulas
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


        public static double ReLU(double x)
        {
            return Math.Max(0, x);
        }

        public static double ReLUdx(double x)
        {
            if (x < 0)
                return 0.0;
            else
                return 1.0;
        }

    }
}
