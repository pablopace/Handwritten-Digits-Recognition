using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NeuralNetwork;

namespace MinistRecognition
{
    public partial class Form1 : Form
    {
        Net net = new Net(0.4, new int[] { 784, 30, 10 });

        MinistReader testFileReader = new MinistReader(MinistReader.Modo.Test);
        //MinistReader trainFileReader = new MinistReader(MinistReader.Modo.Train); // lo abro abajo por ahora 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            int tam = 10;
            int pixels = 28;

            Graphics g = e.Graphics;
            Pen p = new Pen(Color.Black);

            int x = 0, y = 0;

            for (int i = 0; i < pixels; i++)
            {
                for (int j = 0; j < pixels; j++)
                {
                    g.FillRectangle(Brushes.White, x, y, tam, tam);
                    g.DrawRectangle(p, x, y, tam, tam);
                    y += tam;
                }
                y = 0;
                x += tam;
            }
        }


        private void test_Click(object sender, EventArgs e)
        {
            DigitImage di;

            double[] inputs = new double[784];
            double[] outputs = new double[10];

            di = testFileReader.NextDigit();

            for (int i = 0; i < di.pixels.Length; i++)
            {
                for (int j = 0; j < di.pixels[i].Length; j++)
                {
                    inputs[di.pixels.Length * i + j] = di.pixels[i][j];
                }
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i] = 0;
            }
            outputs[di.LabelInt()] = 1;


            DibujarNumero(pictureBox1, label1, inputs, outputs);


            List<Neuron> results = net.Test(new NeuralNetwork.DataSet(inputs, outputs));

            richTextBox1.Text = "";
            int k = 0;
            results.ForEach(n => richTextBox1.Text = richTextBox1.Text + (k++).ToString() + ". " + n.a.ToString("0.0000000000") + "\n");

            DibujarNumero(pictureBox1, label1, di);

        }


        private void train_Click(object sender, EventArgs eargs)
        {   
            MinistReader trainFileReader;
            //DigitImage di;
            double[] inputs = new double[784];
            double[] outputs = new double[10];
            int epocas = int.Parse(textBox1.Text);

            for (int e = 1; e <= epocas; e++)
            {
                trainFileReader = new MinistReader(MinistReader.Modo.Train);

                for (int c = 1; c < 60000; c++) //trainFileReader.numImages
                {
                    DigitImage di = trainFileReader.NextDigit();

                    for (int i = 0; i < di.pixels.Length; i++)
                    {
                        for (int j = 0; j < di.pixels[i].Length; j++)
                        {
                            inputs[di.pixels.Length * i + j] = di.pixels[i][j];
                        }
                    }

                    for (int i = 0; i < outputs.Length; i++)
                    {
                        outputs[i] = 0.01f;
                    }
                    outputs[di.LabelInt()] = 0.99f;

                    //grafico el inputs y veo que es el mismo que output
                    //DibujarNumero(pictureBox1, label1, di);
                    //DibujarNumero(pictureBox1, label1, inputs, outputs);
                    //richTextBox1.Text = di.label.ToString();
                    //richTextBox1.Update();

                    net.Train(new NeuralNetwork.DataSet(inputs, outputs), c, e);
                }

                trainFileReader.Close();
            }

            
        }


        private void DibujarNumero(PictureBox box, Label lab, DigitImage numero)
        {
            int tam = 10;
            int pixels = numero.pixels.Length;

            Bitmap bmp = new Bitmap(box.Width, box.Height);
            Graphics g = Graphics.FromImage(bmp);
            Pen p = new Pen(Color.Black);

            int x = 0, y = 0;

            for (int i = 0; i < pixels; i++)
            {
                for (int j = 0; j < pixels; j++)
                {
                    int v = 255 - (int)Math.Round(numero.pixels[i][j] *255);
                    Color customColor = Color.FromArgb(v, v, v);

                    if (v == 255)
                    {
                        customColor = Color.FromArgb(102, 255, 51);
                    }

                    SolidBrush customBrush = new SolidBrush(customColor);

                    g.FillRectangle(customBrush, x, y, tam, tam);
                    g.DrawRectangle(p, x, y, tam, tam);

                    x += tam;
                }
                x = 0;
                y += tam;
            }

            lab.Text = numero.label.ToString();
            pictureBox1.Image = bmp;
            pictureBox1.Update();
        }



        private void DibujarNumero(PictureBox box, Label lab, double[] inputs, double[] outputs)
        {
            int tam = 10;
            int pixels = 28;

            Bitmap bmp = new Bitmap(box.Width, box.Height);
            Graphics g = Graphics.FromImage(bmp);
            Pen p = new Pen(Color.Black);

            int x = 0, y = 0;

            for (int i = 0; i < pixels; i++)
            {
                for (int j = 0; j < pixels; j++)
                {
                    int v = 255 - (int)inputs[(pixels * i) + j];
                    Color customColor = Color.FromArgb(v, v, v);

                    if (v == 255)
                    {
                        customColor = Color.FromArgb(102, 255, 51);
                    }

                    SolidBrush customBrush = new SolidBrush(customColor);

                    g.FillRectangle(customBrush, x, y, tam, tam);
                    g.DrawRectangle(p, x, y, tam, tam);

                    x += tam;
                }
                x = 0;
                y += tam;
            }

            lab.Text = outputs.Max(v => v).ToString();
            pictureBox1.Image = bmp;
            pictureBox1.Update();
        }




    }
}
