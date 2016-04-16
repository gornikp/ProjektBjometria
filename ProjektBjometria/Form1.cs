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

namespace ProjektBjometria
{
    public partial class Form1 : Form
    {
        public Bitmap picture { get; set; }
        public Bitmap thinnedPicture { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int[,] table;
            int[,] table2;
            //picture = Properties.Resources.odcisk;
            picture = (Bitmap)pictureBox1.Image;
            ThinningLibrary process = new ThinningLibrary();
            table = process.BitmapToTable(picture);
            table2 = process.doZhangSuenThinning(table, false);
            thinnedPicture = process.TableToBitmap(table2);
            picture = (Bitmap)thinnedPicture.Clone();
            PixelFormat eeelo;
            eeelo = thinnedPicture.PixelFormat;
            pictureBox1.Image = picture;
        }
        private void buttonSavePicture_Click(object sender, EventArgs e)
        {
            thinnedPicture.Save("C:\\Users\\BuBu\\Desktop\\02.png");
        }
    }
}
