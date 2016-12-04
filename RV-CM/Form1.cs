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
        List<String> SafefileNames;
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
                                    if (j + k >= 512 || k > 63)
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


        //Nova Snov
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
                SafefileNames = open.SafeFileNames.ToList();
                foreach (String a in SafefileNames)
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
            num = numV;
            return slika1;
        }


        private void listView1_Click(object sender, EventArgs e)
        {
            int index = (sender as ListView).SelectedItems[0].Index;
            pictureBox1.Image = vrnislika(Slike.ElementAt(index));
        }

        private void compressAll_Click(object sender, EventArgs e)
        {
            List<byte> imena_vec = new List<byte>();
            List<String> kompresirano = new List<String>();
            List<byte> zapis = new List<byte>();

            foreach (String ime in SafefileNames)
            {
                imena_vec.Add(Convert.ToByte(ime.Substring(0, ime.Length - 4)));
            }
            //short[,] test  = new short[2, 16] { { -2048, -2048, -2048, -2048, -2048, -2048, -2048, -2048, -2048, -2048, 2047, 2047, 2047,    1, 1775, -2048 }, { -2048, -2048, -2048, -2048, -2048, -2047, -2047, 1775, -1775, -2048, 2047, 2047, -2048, -2048, -2048, -2048 } };
            //short[,] test1 = new short[2, 16] { { -2048, -2048, -2048, -2048, -2048, -2047, -2047,  1775, -1775, -2048, 2047, 2047,-2048,-2048,-2048, -2048 }, { -2048, -2048, -2048, -2048, -2048, -2048, -2048, -2048, -2048, -2048, 2047, 2047, 2047, 1, 1775, -2048 } };
            kompresirano.Add(kompresirana_slika1(Slike[0]));
            //kompresirano.Add(kompresirana_slika1(test));
            for (int i = 1; i < Slike.Count; i++)
            {
            //short[,] Dk = odstej(Slike[i], Slike[i - 1]);
                kompresirano.Add(kompresirana_slika2(Slike[i], Slike[i-1]));
            //kompresirano.Add(kompresirana_slika2(test1, test));
            }
            zapis.Add(Convert.ToByte(8 - (kompresirano.Sum(x => x.Length) % 8)));//izračunaj dolžino vseh stringov ter izračunaj ostanek
            zapis.Add(Convert.ToByte(imena_vec.Count));
            zapis.AddRange(imena_vec);
            string s = "";
            kompresirano.Add(s.PadLeft(zapis[0], '0'));
            s = string.Join("", kompresirano.ToArray());
            zapis.AddRange(GetBytes(s).ToList());
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "CMP3|*.cmp3";
            if (save.ShowDialog() == DialogResult.OK)
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(save.FileName, FileMode.Create)))
                {
                    foreach (byte baj in zapis)
                    {
                        writer.Write(baj);
                    }
                }
            }
        }
        private short[,] odstej(short[,] a, short[,] b)
        {
            short[,] odstej = new short[512, 512];
            for (int i = 0; i < 512; i++)
            {
                for (int j = 0; j < 512; j++)
                {
                    odstej[i, j] = Convert.ToInt16(a[i, j] - b[i, j]);
                }
            }
            return odstej;
        }
        private String kompresirana_slika1(short[,] num1)
        {
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
                                    if (j + k >= 512 || k > 62)
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
            str = "";
            for (int i = 0; i < 512; i++)
            {
                str += array[i];
            }
            return str;
        }
        private String kompresirana_slika2(short[,] a, short[,] b)
        {
            StringBuilder builder = new StringBuilder();
            List<short> Mk_list = new List<short>();
            List<short> Mk_list1 = new List<short>();
            for (int i = 0; i < 512; i++)
            {
                for(int j = 0; j < 512; j++)
                {
                    Mk_list.Add(a[i, j]);
                    Mk_list1.Add(b[i, j]);
                }
            }
            short[] Mk = Mk_list.ToArray();
            short[] Mk1 = Mk_list1.ToArray();
            Mk_list.Clear();
            Mk_list1.Clear();
            for(int i = 0; i < Mk.Length; i++)
            {
                if (Mk[i] - Mk1[i] == 0)
                {
                    builder.Append("11");
                }
                else
                {
                    if ((Mk[i] - Mk1[i]) == (Mk[i - 1] - Mk1[i - 1]))
                    {
                        int j = 0;
                        while ((Mk[i+j] - Mk1[i+j]) == (Mk[i - 1] - Mk1[i - 1]))
                        {
                            j++;
                            if (j > 62)
                                break;
                        }
                        i = i - 1 + j;
                        builder.Append("01");
                        builder.Append(Convert.ToString(j, 2).PadLeft(6, '0'));
                    }
                    else if(((Mk[i] - Mk1[i]) >=-30 && (Mk[i] - Mk1[i]) < 0)|| ((Mk[i] - Mk1[i]) <= 30 && (Mk[i] - Mk1[i]) > 0))
                    {
                        builder.Append("00");
                        if(((Mk[i] - Mk1[i]) >= -2 && (Mk[i] - Mk1[i]) < 2))
                        {
                            builder.Append("00");
                        }
                        else if (((Mk[i] - Mk1[i]) >= -6 && (Mk[i] - Mk1[i]) < 6))
                        {
                            builder.Append("01");
                        }
                        else if (((Mk[i] - Mk1[i]) >= -14 && (Mk[i] - Mk1[i]) < 14))
                        {
                            builder.Append("10");

                        }
                        else if (((Mk[i] - Mk1[i]) >= -30 && (Mk[i] - Mk1[i]) < 30))
                        {
                            builder.Append("11");
                        }
                        builder.Append(kod((short)(Mk[i] - Mk1[i])));
                    }
                    else if(((Mk[i] - Mk1[i]) >= -4095 && (Mk[i] - Mk1[i]) < -30) || ((Mk[i] - Mk1[i]) <= 4095 && (Mk[i] - Mk1[i]) > 30))
                    {
                        builder.Append("10");
                        if (((Mk[i] - Mk1[i]) >= -38 && (Mk[i] - Mk1[i]) <= 38)){
                            builder.Append("00");
                            short stevilo;
                            if ((Mk[i] - Mk1[i]) > 0)
                            {
                                stevilo = (short)(15 - (38 - (Mk[i] - Mk1[i]))); // if(stevilo >= 16/2) x = -(16-1) + 38 + stevilo 
                            }
                            else
                            {
                                stevilo = (short)((Mk[i] - Mk1[i]) + 38); // za dekodiranje if( stevilo < 16/2 ) x = -38 + stevilo
                            }
                            string strin = Convert.ToString(stevilo, 2).PadLeft(4, '0');
                            builder.Append(strin);
                        }
                        else if (((Mk[i] - Mk1[i]) >= -70 && (Mk[i] - Mk1[i]) <= 70))
                        {
                            builder.Append("01");
                            short stevilo;
                            if ((Mk[i] - Mk1[i]) > 0)
                            {
                                stevilo = (short)(63 - (70 - (Mk[i] - Mk1[i]))); // if(stevilo >= 16/2) x = -(16-1) + 38 + stevilo 
                            }
                            else
                            {
                                stevilo = (short)((Mk[i] - Mk1[i]) + 70); // za dekodiranje if( stevilo < 16/2 ) x = -38 + stevilo
                            }
                            string strin = Convert.ToString(stevilo, 2).PadLeft(6, '0');
                            builder.Append(strin);
                        }
                        else if (((Mk[i] - Mk1[i]) >= -326 && (Mk[i] - Mk1[i]) <= 326))
                        {
                            builder.Append("10");
                            short stevilo;
                            if ((Mk[i] - Mk1[i]) > 0)
                            {
                                stevilo = (short)(511 - (326 - (Mk[i] - Mk1[i]))); // if(stevilo >= 16/2) x = -(16-1) + 38 + stevilo 
                            }
                            else
                            {
                                stevilo = (short)((Mk[i] - Mk1[i]) + 326); // za dekodiranje if( stevilo < 16/2 ) x = -38 + stevilo
                            }
                            string strin = Convert.ToString(stevilo, 2).PadLeft(9, '0');
                            builder.Append(strin);
                        }
                        else if (((Mk[i] - Mk1[i]) >= -4095 && (Mk[i] - Mk1[i]) <= 4095))
                        {
                            builder.Append("11");
                            short stevilo;
                            if ((Mk[i] - Mk1[i]) > 0)
                            {
                                stevilo = (short)(7537 - (4095 - (Mk[i] - Mk1[i]))); // if(stevilo >= 16/2) x = -(16-1) + 38 + stevilo 
                            }
                            else
                            {
                                stevilo = (short)((Mk[i] - Mk1[i]) + 4095); // za dekodiranje if( stevilo < 16/2 ) x = -38 + stevilo
                            }
                            string strin = Convert.ToString(stevilo, 2).PadLeft(13, '0');
                            builder.Append(strin);
                        }
                    }
                }
            }
            return builder.ToString();
        }

        private void DecompressAll_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "CMP3|*.cmp3";
            byte[] bajt;
            if (open.ShowDialog() == DialogResult.OK)
            {
                bajt = File.ReadAllBytes(open.FileName);
                int ostanek = bajt[0];
                int stevilo_slik = bajt[1];
                byte[] imena_slik = bajt.Take(2 + stevilo_slik).Skip(2).ToArray();
                byte[] bajtiOst = bajt.Skip(2 + stevilo_slik).ToArray();
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bajtiOst.Length; i++)
                {
                    string str = Convert.ToString(bajtiOst[i], 2);

                    str = str.PadLeft(8, '0');
                    builder.Append(str);
                }
                string bitstring = builder.ToString();
                List<short[]> slikeOdstete = decompress_all(bitstring.Remove(bitstring.Length-ostanek).ToString(), stevilo_slik);
                SaveFileDialog save = new SaveFileDialog();
                if (save.ShowDialog() == DialogResult.OK)
                {
                    string savefile = save.FileName.Remove(save.FileName.LastIndexOf('\\')+1);
                    int k = 0;
                    foreach (short[] element in slikeOdstete)
                    {
                        using (BinaryWriter writer = new BinaryWriter(File.Open(savefile+imena_slik[k].ToString().PadLeft(4,'0')+".img", FileMode.Create)))
                        {
                            for (int i = 0; i < 512; i++)
                                for (int j = 0; j < 512; j++)
                                {
                                    writer.Write(Convert.ToInt16(element[j * 512 + i]));
                                }
                        }
                        k++;
                    }
                }
            }

        }
        private List<short[]> decompress_all(string bits, int steviloSlik)
        {
            List<short[]> shorti = new List<short[]>();
            List<short> slika = new List<short>();
            int chunkSize = 2;
            int ponovitev=0;
            for (int i = 0; i < bits.Length; i += chunkSize)
            {
                
                if (ponovitev < 1)
                {
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
                    if (slika.Count > 262143)
                    {
                        slika.Count();
                    }
                    if (slika.Count == 512)
                    {
                        shorti.Add(slika.ToArray());
                        slika.Clear();
                    }

                }
                else //KODA ZA ostale slike!
                {
                    if (i + chunkSize > bits.Length)
                        break;
                    string chunk = bits.Substring(i, chunkSize);

                    if (chunk == "11")
                    {
                        slika.Add((short)(0+(shorti.Last().ElementAt(slika.Count()))));
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
                            slika.Add((short)(array[index] + (shorti.Last().ElementAt(slika.Count()))));
                            i += 4;
                        }
                        //-6,6
                        else if (bits.Substring(i + 2, 2) == "01")
                        {
                            Int16 index = Convert.ToInt16(bits.Substring(i + 2 + 2, 3), 2);
                            short[] array = new short[] { -6, -5, -4, -3, 3, 4, 5, 6 };
                            slika.Add((short)(array[index] + (shorti.Last().ElementAt(slika.Count()))));
                            i += 5;
                        }
                        //-14,14
                        else if (bits.Substring(i + 2, 2) == "10")
                        {

                            Int16 index = Convert.ToInt16(bits.Substring(i + 2 + 2, 4), 2);
                            short[] array = new short[] { -14, -13, -12, -11, -10, -9, -8, -7, 7, 8, 9, 10, 11, 12, 13, 14 };
                            slika.Add((short)(array[index] + (shorti.Last().ElementAt(slika.Count()))));
                            i += 6;
                        }
                        //-30,30
                        else if (bits.Substring(i + 2, 2) == "11")
                        {
                            Int16 index = Convert.ToInt16(bits.Substring(i + 2 + 2, 5), 2);
                            short[] array = new short[] { -30, -29, -28, -27, -26, -25, -24, -23, -22, -21, -20, -19, -18, -17, -16, -15, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
                            slika.Add((short)(array[index] + (shorti.Last().ElementAt(slika.Count()))));
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
                        short x;
                        if (bits.Substring(i + 2, 2) == "00")
                        {
                            Int16 index = Convert.ToInt16(bits.Substring(i + 2 + 2, 4), 2);
                            if (index >= 16 / 2) x = (short)(-(16 - 1) + 38 + index);
                            else x = (short)(-38 + index);
                            slika.Add((short)(x+ (shorti.Last().ElementAt(slika.Count()))));
                            i += 6;
                        }
                        else if (bits.Substring(i + 2, 2) == "01")
                        {
                            Int16 index = Convert.ToInt16(bits.Substring(i + 2 + 2, 6), 2);
                            if (index >= 64 / 2) x = (short)(-(64 - 1) + 70 + index);
                            else x = (short)(-70 + index);
                            slika.Add((short)(x+ (shorti.Last().ElementAt(slika.Count()))));
                            i += 8;
                        }
                        else if (bits.Substring(i + 2, 2) == "10")
                        {
                            Int16 index = Convert.ToInt16(bits.Substring(i + 2 + 2, 9), 2);
                            if (index >= 512 / 2) x = (short)(-(512 - 1) + 326 + index);
                            else x = (short)(-326 + index);
                            slika.Add((short)(x+ (shorti.Last().ElementAt(slika.Count()))));
                            i += 11;
                        }
                        else if (bits.Substring(i + 2, 2) == "11")
                        {
                            Int16 index = Convert.ToInt16(bits.Substring(i + 2 + 2, 13), 2);
                            if (index >= 7538 / 2) x = (short)(-(7538 - 1) + 4095 + index);
                            else x = (short)(-4095 + index);
                            slika.Add((short)(x+ (shorti.Last().ElementAt(slika.Count()))));
                            i += 15;
                        }
                        if (slika.Count == 14073)
                        {
                            i = i;
                        }
                    }
                    
                    if (slika.Count == 262144)
                    {
                        shorti.Add(slika.ToArray());
                        slika.Clear();
                    }
                }
                ponovitev = shorti.Count;
            }
            return shorti;
        }
        private short[] pristej(short[] a, short[] b) //a = slika odstete, b = original
        {
            short[] odstej = new short[512*512];
            for (int i = 0; i < 512*512; i++)
            {
                
                    odstej[i] = Convert.ToInt16(a[i] + b[i]);
                
            }
            return odstej;
        }
    }
}
/*else // ABSOLUTNO!
                        {
                            str = str + "10";
                            short stevilo;
                            if (num[i, j] > 4095)
                                stevilo=1;
                            if (Convert.ToInt32(num1[i, j]) >= -38 && Convert.ToInt32(num1[i, j]) <= 38) //16 stevil
                            {
                                str += "00";
                                if (num1[i, j] > 0)
                                {
                                    stevilo = (short)(15 - (38 - num1[i, j])); // if(stevilo >= 16/2) x = -(16-1) + 38 + stevilo 
                                }
                                else
                                {
                                    stevilo = (short)(num1[i, j] + 38); // za dekodiranje if( stevilo < 16/2 ) x = -38 + stevilo
                                }
                                if (stevilo > 15)
                                    stevilo = stevilo;
                                string strin = Convert.ToString(stevilo, 2).PadLeft(4, '0');
                                str += strin;
                            }
                            else if (Convert.ToInt32(num1[i, j]) >= -70 && Convert.ToInt32(num1[i, j]) <= 70) //64 stevil
                            {
                                str += "01";
                                if (num1[i, j] > 0)
                                {
                                    stevilo = (short)(63 - (70 - num1[i, j])); // if(stevilo >= 64/2) x = -(64-1) + 70 + stevilo 
                                }
                                else
                                {
                                    stevilo = (short)(num1[i, j] + 70); // za dekodiranje if( stevilo < 64/2 ) x = -70 + stevilo
                                }
                                if (stevilo > 63)
                                    stevilo = stevilo;
                                string strin = Convert.ToString(stevilo, 2).PadLeft(6, '0');
                                str += strin;

                            }
                            else if (Convert.ToInt32(num1[i, j]) >= -326 && Convert.ToInt32(num1[i, j]) <= 326) //512 stevil
                            {
                                str += "10";
                                if (num1[i, j] > 0)
                                {
                                    stevilo = (short)(511 - (326 - num1[i, j])); // if(stevilo >= 512/2) x = -(512-1) + 326 + stevilo 
                                }
                                else
                                {
                                    stevilo = (short)(num1[i, j] + 326); // za dekodiranje if( stevilo < 512/2 ) x = -326 + stevilo
                                }
                                if (stevilo > 511)
                                    stevilo = stevilo;
                                string strin = Convert.ToString(stevilo, 2).PadLeft(9, '0');
                                str += strin;
                            }
                            else if (Convert.ToInt32(num1[i, j]) >= -4095 && Convert.ToInt32(num1[i, j]) <= 4095) //7538 stevil
                            {
                                str += "11";
                                if (num1[i, j] > 0)
                                {
                                    stevilo = (short)(7537 - (4095 - num1[i, j])); // if(stevilo >= 7538/2) x = -(7538-1) + 4095 + stevilo 
                                }
                                else
                                {
                                    stevilo = (short)(num1[i, j] + 4095); // za dekodiranje if( stevilo < 7538/2 ) x = -4095 + stevilo
                                }
                                if (stevilo > 7537)
                                    stevilo = stevilo;
                                string strin = Convert.ToString(stevilo, 2).PadLeft(13, '0');
                                str += strin;

                            }

                        }
*/