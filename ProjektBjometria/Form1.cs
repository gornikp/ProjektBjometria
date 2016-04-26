﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ProjektBjometria
{
    public partial class Form1 : Form
    {
        public Bitmap Picture {
            get
            {
                return picture;
            }
            set
            {
                picture = value;
                pictureBox1.Image = picture;
            }
        }
        private Bitmap picture;
        public Bitmap thinnedPicture { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            picture = Properties.Resources.odcisk;
            picture = (Bitmap)pictureBox1.Image;
            ThinningLibrary process = new ThinningLibrary();
            //table = process.BitmapToTable(picture);
            //table2 = process.doZhangSuenThinning(table, false);
            //thinnedPicture = process.TableToBitmap(table2);
            picture = process.TransformOtsu(picture);
            thinnedPicture = process.processImage((Bitmap)picture.Clone());
            picture = (Bitmap)thinnedPicture.Clone();
            pictureBox1.Image = picture;
        }
        private void buttonSavePicture_Click(object sender, EventArgs e)
        {
            thinnedPicture.Save("C:\\Users\\Monika\\Documents\\STUDIA\\sem6\\Podstawy biometrii\\odcisk.jpg");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            MinutiaFinder minutiaFinder = new MinutiaFinder(picture);
            minutiaFinder.findCrosscuts();
            picture = minutiaFinder.result;
            pictureBox1.Image = picture;
        }
        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Picture = new Bitmap(((string[])e.Data.GetData(DataFormats.FileDrop, false))[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("podano błędny plik " + ex.Message);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            
        }
    }
}
