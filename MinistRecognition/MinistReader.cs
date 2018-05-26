using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinistRecognition
{
    public class MinistReader
    {
        private const string testLabels = "t10k-labels.idx1-ubyte";
        private const string testImages = "t10k-images.idx3-ubyte";
        private const string trainLabels = "train-labels.idx1-ubyte";
        private const string trainImages = "train-images.idx3-ubyte";
        public enum Modo { Test, Train };
        string labelsFile;
        string imagesFile;
        private static string filedir = System.IO.Path.GetFullPath(@"D:\VisualStudioProjects\MnistFiles\");

        int magic1;
        public int numImages;
        int numRows;
        int numCols;

        int magic2;
        int numLabels;

        FileStream ifsLabels;
        FileStream ifsImages;

        BinaryReader brLabels;
        BinaryReader brImages;


        public MinistReader(Modo m)
        {
            if (m == Modo.Train)
            {
                labelsFile = trainLabels;
                imagesFile = trainImages;
            }
            else
            {
                labelsFile = testLabels;
                imagesFile = testImages;
            }

            //Console.WriteLine("\nBegin\n");
            ifsLabels = new FileStream(filedir + labelsFile, FileMode.Open); 
            ifsImages = new FileStream(filedir + imagesFile, FileMode.Open); 

            brLabels = new BinaryReader(ifsLabels);
            brImages = new BinaryReader(ifsImages);

            magic1 = SwapEndianness(brImages.ReadInt32()); // discard
            numImages = SwapEndianness(brImages.ReadInt32());
            numRows = SwapEndianness(brImages.ReadInt32());
            numCols = SwapEndianness(brImages.ReadInt32());

            magic2 = SwapEndianness(brLabels.ReadInt32());
            numLabels = SwapEndianness(brLabels.ReadInt32());

            if (numImages != numLabels) throw new System.Exception("La cantidad de imagenes es distinta a la cant de labels");
        }


        public void Close()
        {
            ifsImages.Close();
            brImages.Close();
            ifsLabels.Close();
            brLabels.Close();
        }


        public DigitImage NextDigit()
        {
            /*byte[][] pixels = new byte[28][];
            for (int i = 0; i < pixels.Length; ++i)
                pixels[i] = new byte[28];*/

            byte[][] pixels = new byte[28][];

            for (int i = 0; i < 28; ++i)
            {
                pixels[i] = new byte[28];

                for (int j = 0; j < 28; ++j)
                {
                    pixels[i][j] = brImages.ReadByte();
                }
            }

            byte lbl = brLabels.ReadByte();

            DigitImage dImage = new DigitImage(pixels, lbl);
            return dImage;
        }


        private static int SwapEndianness(int value)
        {
            var b1 = (value >> 0) & 0xff;
            var b2 = (value >> 8) & 0xff;
            var b3 = (value >> 16) & 0xff;
            var b4 = (value >> 24) & 0xff;

            return b1 << 24 | b2 << 16 | b3 << 8 | b4 << 0;
        }

    }


    public class DigitImage
    {
        public byte[][] pixels;
        public byte label;

        public DigitImage(byte[][] pixels, byte label)
        {
            this.pixels = new byte[28][];
            for (int i = 0; i < this.pixels.Length; ++i)
                this.pixels[i] = new byte[28];

            for (int i = 0; i < 28; ++i)
                for (int j = 0; j < 28; ++j)
                    this.pixels[i][j] = pixels[i][j];

            this.label = label;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < 28; ++i)
            {
                for (int j = 0; j < 28; ++j)
                {
                    if (this.pixels[i][j] == 0)
                        s += " "; // white
                    else if (this.pixels[i][j] == 255)
                        s += "O"; // black
                    else
                        s += "."; // gray
                }
                s += "\n";
            }
            s += this.label.ToString();
            return s;
        } // ToString

        public int LabelInt()
        {
            return int.Parse(label.ToString());
        }

    }


}

