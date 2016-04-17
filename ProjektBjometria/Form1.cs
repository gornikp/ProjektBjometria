using System;
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
            thinnedPicture = process.processImage((Bitmap)picture.Clone());
            picture = (Bitmap)thinnedPicture.Clone();
            pictureBox1.Image = picture;
        }
        private void buttonSavePicture_Click(object sender, EventArgs e)
        {
            thinnedPicture.Save("C:\\Users\\BuBu\\Desktop\\02.png");
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

        private void button2_Click(object sender, EventArgs e)
        {
            int[,] table;
            int[,] table2;
            picture = Properties.Resources.odcisk;
            picture = (Bitmap)pictureBox1.Image;
            ThinningLibrary process = new ThinningLibrary();
            table = process.BitmapToTable(picture);
            table2 = process.doZhangSuenThinning(table, false);
            thinnedPicture = process.TableToBitmap(table2);          
            picture = (Bitmap)thinnedPicture.Clone();          
            pictureBox1.Image = picture;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
    }
}
