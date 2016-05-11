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
        int zoom { get; set; }
        public Bitmap Picture {
            get
            {
                return picture;
            }
            set
            {
                picture = value;
            }
        }
        private Bitmap picture;
        public Bitmap thinnedPicture { get; set; }
        public Form1()
        {
            zoom = 1;
            picture = Properties.Resources.odcisk;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            picture = Properties.Resources.odcisk;
            //picture = (Bitmap)pictureBox1.Image;
            ThinningLibrary process = new ThinningLibrary();
            picture = process.TransformOtsu(picture);
            thinnedPicture = process.processImage((Bitmap)picture.Clone());
            picture = (Bitmap)thinnedPicture.Clone();
            pictureBox1.Image = picture;
        }
        private void buttonSavePicture_Click(object sender, EventArgs e)
        {
            picture.Save("C:\\Users\\Monika\\Documents\\STUDIA\\sem6\\Podstawy biometrii\\odcisk.jpg");
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

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (zoom == 1)
            {
                zoom = 3;
                pictureBox1.Image = new Bitmap(picture, picture.Width * zoom, picture.Height * zoom);
            }
            else
            {
                zoom = 1;
                pictureBox1.Image = new Bitmap(picture, picture.Width * zoom, picture.Height * zoom);
            }
        }
        private static bool[] buildKillsArray(int[] kills)
        {
            bool[] ar = new bool[256];
            ar = Enumerable.Repeat(true, 256).ToArray();
            for (int i = 0; i < kills.Length; ++i)
                ar[kills[i]] = true;
            return ar;
        }
        private static bool[] killsRound = buildKillsArray(new int[]{
			3, 12,  48, 192, 6, 24,  96, 129,	//	-	2 sasiadow
			14, 56, 131, 224, 7, 28, 112, 193,	//	-	3 sasiadow
			195, 135, 15, 30, 60, 120, 240, 225,//	-	4 sasiadow
//			31, 62, 124, 248, 241, 227, 199, 143,//	-	5 sasiadow
//			63, 126, 252, 249, 243, 231, 207, 159,//-	6 sasiadow
//			254, 253, 251, 247, 239, 223, 190, 127,//-	7 sasiadow
		});
    }
}
