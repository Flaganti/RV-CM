using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RV_CM
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        short[,] num = new short[512, 512];
        Bitmap slika = new Bitmap(512, 512);
        List<short[,]> Slike = new List<short[,]>();
        List<String> fileNames;
        private void button1_Click(object sender, EventArgs e) //NALOŽI IMG
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Img|*.img";
            DialogResult res = open.ShowDialog();
            if (res == DialogResult.OK)
            {
                BinaryReader br1 = new BinaryReader(File.Open(open.FileName, FileMode.Open));
                for (int i = 0; i < 512; i++)
                    for (int j = 0; j < 512; j++)
                    {
                        num[j, i] = (short)br1.ReadInt16();
                    }
                br1.Close();
                for (int i = 0; i < 512; i++)
                    for (int j = 0; j < 512; j++)
                    {
                        double a = num[i, j] + 2048;
                        a = a / 4096;
                        int c = Convert.ToInt32(a * 255);
                        Color col = Color.FromArgb(c, c, c);
                        slika.SetPixel(i, j, col);

                    }
                pictureBox1.Image = slika;
            }


        }

        private void button2_Click(object sender, EventArgs e)//NALOŽI LUT
        {
            byte[,] lut = new byte[3, 256];
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Lut|*.lut";
            DialogResult res = open.ShowDialog();
            if (res == DialogResult.OK)
            {
                BinaryReader br1 = new BinaryReader(File.Open(open.FileName, FileMode.Open));
                for (int i = 0; i < 256; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        lut[j, i] = br1.ReadByte();
                    }
                br1.Close();
                for (int i = 0; i < 512; i++)
                    for (int j = 0; j < 512; j++)
                    {
                        double a = num[i, j] + 2048;
                        a = a / 4096;
                        int c = Convert.ToInt32(a * 255);
                        Color col = Color.FromArgb(Convert.ToInt32(lut[2, c]), Convert.ToInt32(lut[1, c]), Convert.ToInt32(lut[0, c]));
                        slika.SetPixel(i, j, col);
                    }
                pictureBox1.Image = slika;
            }
        }

        private void button3_Click(object sender, EventArgs e) //COMPRESIJA
        {

            short[,] num1 = new short[512, 512];
            //short[,] num1 = new short[1, 16] { { -2048, -2048, -2048, 55, 53, 53, 53, 1000, 100, 99, 75, 49, 2047, 11, -2047, -2048 } };
            //short[,] num1 = new short[2, 104] { { 5, -2048, -2048, -2048, 55, 53, 53, 53, 53, 51, 51, 51, 10, 11, 11, 11, -2048, 200, 200, 1000, 100, 99, 99, 75, 75, 49, 49, 10, 8, 8, 8, 8, 8, 1, 1, 1, 1, 1, -2048, 1, 1, 1, 99, 99, 40, 35, 35, 25, 25, 533, 529, 520, 5, -2048, -2048, -2048, 55, 53, 53, 53, 53, 51, 51, 51, 10, 11, 11, 11, -2048, 200, 200, 1000, 100, 99, 99, 75, 75, 49, 49, 10, 8, 8, 8, 8, 8, 1, 1, 1, 1, 1, -2048, 1, 1, 1, 99, 99, 40, 35, 35, 25, 25, 533, 529, 520 }, { 5, -2048, -2048, -2048, 55, 53, 53, 53, 53, 51, 51, 51, 10, 11, 11, 11, -2048, 200, 200, 1000, 100, 99, 99, 75, 75, 49, 49, 10, 8, 8, 8, 8, 8, 1, 1, 1, 1, 1, -2048, 1, 1, 1, 99, 99, 40, 35, 35, 25, 25, 533, 529, 520, 5, -2048, -2048, -2048, 55, 53, 53, 53, 53, 51, 51, 51, 10, 11, 11, 11, -2048, 200, 200, 1000, 100, 99, 99, 75, 75, 49, 49, 10, 8, 8, 8, 8, 8, 1, 1, 1, 1, 1, -2048, 1, 1, 1, 99, 99, 40, 35, 35, 25, 25, 533, 529, 520 } };
            num1 = num;
            string[] array = new string[512];
            string str = "";
            for (int i = 0; i < 512; i++)
            {
                for (int j = 0; j < 512; j++)
                {

                    //11-zrak
                    if (num1[i, j] == -2048)
                    {
                        str = str + "11";
                    }
                    //00-razlika 01-ponovitev 10-absolutno
                    else if (j > 0)
                    {
                        if (((num1[i, j] - num1[i, j - 1]) <= 30) && ((num1[i, j] - num1[i, j - 1]) >= -30))
                        {
                            //ponovitev
                            if ((num1[i, j] - num1[i, j - 1]) == 0)
                            {
                                str = str + "01";
                                int k = 0;
                                while (num1[i, j + k] == num1[i, j - 1])
                                {
                                    k++;
                                    if (j + k >= 512)
                                        break;
                                }
                                string strin = Convert.ToString(k, 2);
                                for (int l = 0; l < 6 - strin.Length; l++)
                                {
                                    str = str + "0";
                                }
                                str = str + strin;
                                j = j - 1 + k;
                            }
                            //razlika
                            else
                            {
                                //boli me kurac
                                str = str + "00";
                                //00 - [-2,-1][1,2]
                                if (num1[i, j] - num1[i, j - 1] >= -2 && num1[i, j] - num1[i, j - 1] <= 2)
                                {
                                    str = str + "00";
                                }
                                //01 - [-6,-3][3,6] 
                                else if (num1[i, j] - num1[i, j - 1] >= -6 && num1[i, j] - num1[i, j - 1] <= 6)
                                {
                                    str = str + "01";
                                }
                                //10 - [-14,-7][7,14]
                                else if (num1[i, j] - num1[i, j - 1] >= -14 && num1[i, j] - num1[i, j - 1] <= 14)
                                {
                                    str = str + "10";
                                }
                                //11 - [-30,-15][15,30]
                                else if (num1[i, j] - num1[i, j - 1] >= -30 && num1[i, j] - num1[i, j - 1] <= 30)
                                {
                                    str = str + "11";
                                }
                                str = str + kod(Convert.ToInt16(num1[i, j] - num1[i, j - 1]));
                            }
                        }
                        else
                        {
                            str = str + "10";
                            string strin = "";
                            if (num1[i, j] < 0)
                            {
                                strin = Convert.ToString(num1[i, j] * (-1), 2);
                                str += "1";
                                for (int l = 0; l < 11 - strin.Length; l++)
                                {
                                    str = str + "0";
                                }

                            }
                            else
                            {
                                strin = Convert.ToString(num1[i, j], 2);
                                for (int l = 0; l < 12 - strin.Length; l++)
                                {
                                    str = str + "0";
                                }
                            }

                            str = str + strin;
                        }
                    }
                    else
                    {
                        str = str + "10";
                        string strin = "";
                        if (num1[i, j] < 0)
                        {
                            strin = Convert.ToString(num1[i, j] * (-1), 2);
                            str += "1";
                            for (int l = 0; l < 11 - strin.Length; l++)
                            {
                                str = str + "0";
                            }

                        }
                        else
                        {
                            strin = Convert.ToString(num1[i, j], 2);
                            for (int l = 0; l < 12 - strin.Length; l++)
                            {
                                str = str + "0";
                            }
                        }

                        str = str + strin;
                    }

                }
                array[i] = str;
                str = "";
            }
            int bit = 0;
            for (int i = 0; i < 512; i++)
            {
                bit += array[i].Length;
            }

            int ostanek = 8 - (bit % 8);
            //  int bajt = (bit + ostanek) / 8;
            str = "";
            for (int i = 0; i < 512; i++)
            {
                str += array[i];
            }
            MessageBox.Show(str.Length.ToString());
            for (int i = 0; i < ostanek; i++)
            {
                str += "0";
            }
            //MessageBox.Show(bit.ToString() + "          " + (bajt / 1024).ToString() + "KB");
            byte ost = Convert.ToByte(ostanek);
            List<byte> bajti = GetBytes(str).ToList<byte>();
            bajti.Insert(0, ost);
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "CMP|*.cmp";
            if (save.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(save.FileName, bajti.ToArray());
            //MessageBox.Show(bajti.Length.ToString());
        }
        private string kod(short i)
        {
            string str = "";
            switch (i)
            {
                case (-2):
                    str = "00";
                    break;
                case (-1):
                    str = "01";
                    break;
                case (1):
                    str = "10";
                    break;
                case (2):
                    str = "11";
                    break;
                /////////////
                case (-6):
                    str = "000";
                    break;
                case (-5):
                    str = "001";
                    break;
                case (-4):
                    str = "010";
                    break;
                case (-3):
                    str = "011";
                    break;
                case (3):
                    str = "100";
                    break;
                case (4):
                    str = "101";
                    break;
                case (5):
                    str = "110";
                    break;
                case (6):
                    str = "111";
                    break;
                /////////////
                case (-14):
                    str = "0000";
                    break;
                case (-13):
                    str = "0001";
                    break;
                case (-12):
                    str = "0010";
                    break;
                case (-11):
                    str = "0011";
                    break;
                case (-10):
                    str = "0100";
                    break;
                case (-9):
                    str = "0101";
                    break;
                case (-8):
                    str = "0110";
                    break;
                case (-7):
                    str = "0111";
                    break;
                case (7):
                    str = "1000";
                    break;
                case (8):
                    str = "1001";
                    break;
                case (9):
                    str = "1010";
                    break;
                case (10):
                    str = "1011";
                    break;
                case (11):
                    str = "1100";
                    break;
                case (12):
                    str = "1101";
                    break;
                case (13):
                    str = "1110";
                    break;
                case (14):
                    str = "1111";
                    break;
                /////////////
                case (-30):
                    str = "00000";
                    break;
                case (-29):
                    str = "00001";
                    break;
                case (-28):
                    str = "00010";
                    break;
                case (-27):
                    str = "00011";
                    break;
                case (-26):
                    str = "00100";
                    break;
                case (-25):
                    str = "00101";
                    break;
                case (-24):
                    str = "00110";
                    break;
                case (-23):
                    str = "00111";
                    break;
                case (-22):
                    str = "01000";
                    break;
                case (-21):
                    str = "01001";
                    break;
                case (-20):
                    str = "01010";
                    break;
                case (-19):
                    str = "01011";
                    break;
                case (-18):
                    str = "01100";
                    break;
                case (-17):
                    str = "01101";
                    break;
                case (-16):
                    str = "01110";
                    break;
                case (-15):
                    str = "01111";
                    break;
                case (15):
                    str = "10000";
                    break;
                case (16):
                    str = "10001";
                    break;
                case (17):
                    str = "10010";
                    break;
                case (18):
                    str = "10011";
                    break;
                case (19):
                    str = "10100";
                    break;
                case (20):
                    str = "10101";
                    break;
                case (21):
                    str = "10110";
                    break;
                case (22):
                    str = "10111";
                    break;
                case (23):
                    str = "11000";
                    break;
                case (24):
                    str = "11001";
                    break;
                case (25):
                    str = "11010";
                    break;
                case (26):
                    str = "11011";
                    break;
                case (27):
                    str = "11100";
                    break;
                case (28):
                    str = "11101";
                    break;
                case (29):
                    str = "11110";
                    break;
                case (30):
                    str = "11111";
                    break;
            }
            return str;
        }
        public static byte[] GetBytes(string bitString)
        {
            return Enumerable.Range(0, bitString.Length / 8).
                Select(pos => Convert.ToByte(
                    bitString.Substring(pos * 8, 8),
                    2)
                ).ToArray();
        }

        private void button4_Click(object sender, EventArgs e)//DEKOMPRESIJA
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "CMP|*.cmp";
            DialogResult res = open.ShowDialog();
            byte[] vsiBajti;
            if (res == DialogResult.OK)
            {
                vsiBajti = File.ReadAllBytes(open.FileName);
                byte[] bajtiOst = vsiBajti.Skip(1).ToArray();
                byte ost = vsiBajti[0];
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bajtiOst.Length; i++)
                {
                    string str = Convert.ToString(bajtiOst[i], 2);

                    if (str.Length < 8)
                    {
                        for (int a = 0; a < 8 - str.Length; a++)
                        {
                            builder.Append("0");
                        }
                    }
                    builder.Append(str);
                }
                string biti = builder.ToString();
                MessageBox.Show((biti.Length - ost).ToString());
                short[] slika = decompress(biti.Remove(biti.Length - ost));
                MessageBox.Show(slika.Length.ToString());
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "IMG|*.img";
                if (save.ShowDialog() == DialogResult.OK)
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open(save.FileName, FileMode.Create)))
                    {
                        for (int i = 0; i < 512; i++)
                            for (int j = 0; j < 512; j++/*slika.Length; j+=Convert.ToInt32(Math.Sqrt(slika.Length))*/)
                            {
                                writer.Write(Convert.ToInt16(slika[j * 512 + i]));
                            }
                    }
                    /* using (FileStream fs = new FileStream(save.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                     {
                         using (BinaryWriter bw = new BinaryWriter(fs))
                         {
                             // Write the number of items
                             //bw.Write(slika.Length);

                             foreach (short value in slika)
                             {
                                 bw.Write(Convert.ToInt16(value));
                             }
                         }
                     }*/

                }
            }
        }
        private short[] decompress(string bits)
        {

            List<short> slika = new List<short>();
            int chunkSize = 2;
            for (int i = 0; i < bits.Length; i += chunkSize)
            {
                /* if (slika.Count>0)
                 {
                     if(slika.Last()>2047||slika.Last()<-2048)
                         MessageBox.Show("error"+i.ToString());
                 }*/
                if (i + chunkSize > bits.Length)
                    break;
                string chunk = bits.Substring(i, chunkSize);

                if (chunk == "11")
                {
                    slika.Add(-2048);
                }
                //razlika
                else if (chunk == "00")
                {
                    //TODO: Pretvori index v vrednost! ter odštej pri;
                    //-2,2
                    if (bits.Substring(i + 2, 2) == "00")
                    {
                        Int16 index = Convert.ToInt16(bits.Substring(i + 2 + 2, 2), 2);
                        short[] array = new short[] { -2, -1, 1, 2 };
                        short stevilo = Convert.ToInt16(slika.Last() + array[index]);
                        slika.Add(stevilo);
                        i += 4;
                    }
                    //-6,6
                    else if (bits.Substring(i + 2, 2) == "01")
                    {
                        Int16 index = Convert.ToInt16(bits.Substring(i + 2 + 2, 3), 2);
                        short[] array = new short[] { -6, -5, -4, -3, 3, 4, 5, 6 };
                        short stevilo = Convert.ToInt16(slika.Last() + array[index]);
                        slika.Add(stevilo);
                        i += 5;
                    }
                    //-14,14
                    else if (bits.Substring(i + 2, 2) == "10")
                    {

                        Int16 index = Convert.ToInt16(bits.Substring(i + 2 + 2, 4), 2);
                        short[] array = new short[] { -14, -13, -12, -11, -10, -9, -8, -7, 7, 8, 9, 10, 11, 12, 13, 14 };
                        short stevilo = Convert.ToInt16(slika.Last() + array[index]);
                        slika.Add(stevilo);
                        i += 6;
                    }
                    //-30,30
                    else if (bits.Substring(i + 2, 2) == "11")
                    {
                        Int16 index = Convert.ToInt16(bits.Substring(i + 2 + 2, 5), 2);
                        short[] array = new short[] { -30, -29, -28, -27, -26, -25, -24, -23, -22, -21, -20, -19, -18, -17, -16, -15, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
                        short stevilo = Convert.ToInt16(slika.Last() + array[index]);
                        slika.Add(stevilo);
                        i += 7;
                    }
                }
                else if (chunk == "01")
                {
                    Int16 ponovitve = Convert.ToInt16(bits.Substring(i + 2, 6), 2);
                    short stevilo = slika.Last();
                    for (int a = 0; a < ponovitve; a++)
                    {
                        slika.Add(stevilo);
                    }
                    i += 6;
                }
                else if (chunk == "10")
                {
                    //Branje negativnih vrednosti!!!!
                    if (bits[i + 2] == '1')
                    {
                        string sub = bits.Substring(i + 3, 11);
                        Int16 stevilo = Convert.ToInt16(sub, 2);
                        slika.Add(Convert.ToInt16(stevilo * (-1)));
                        i += 12;
                    }
                    else
                    {
                        string sub = bits.Substring(i + 2, 12);
                        Int16 stevilo = Convert.ToInt16(sub, 2);
                        slika.Add(stevilo);
                        i += 12;
                    }
                }
            }
            return slika.ToArray();
        }

        private void vecSlik_Click(object sender, EventArgs e)
        {
            Slike.Clear();
            listView1.Clear();
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Img|*.img";
            open.Multiselect = true;
            DialogResult res = open.ShowDialog();
            if (res == DialogResult.OK)
            {            
                fileNames = open.FileNames.ToList();
                foreach(String a in open.SafeFileNames)
                {
                    listView1.Items.Add(a);
                }
                for (int k = 0; k < fileNames.Count; k++)
                {
                    BinaryReader br1 = new BinaryReader(File.Open(fileNames[k], FileMode.Open));
                    short[,] numV = new short[512, 512];
                    for (int i = 0; i < 512; i++)
                        for (int j = 0; j < 512; j++)
                        {
                            numV[j, i] = (short)br1.ReadInt16();
                        }
                    Slike.Add(numV);
                    br1.Close();
                }
            }
            pictureBox1.Image = vrnislika(Slike[0]);
        }
        private Bitmap vrnislika(short[,] numV)
        {
            Bitmap slika1 = new Bitmap(512, 512);
            for (int i = 0; i < 512; i++)
                for (int j = 0; j < 512; j++)
                {
                    double a = numV[i, j] + 2048;
                    a = a / 4096;
                    int c = Convert.ToInt32(a * 255);
                    Color col = Color.FromArgb(c, c, c);
                    slika1.SetPixel(i, j, col);

                }
            return slika1;
        }


        private void listView1_Click(object sender, EventArgs e)
        {
            int index = (sender as ListView).SelectedItems[0].Index;
            pictureBox1.Image = vrnislika(Slike.ElementAt(index));
        }
    }
}
