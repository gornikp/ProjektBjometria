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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int[,] table;
            int[,] table2;
            Bitmap elo = new Bitmap("C:\\Users\\BuBu\\Desktop\\01.png");
            PixelFormat eeelo = elo.PixelFormat;
            ThinningLibrary process = new ThinningLibrary();
            table = process.BitmapToTable(elo);
            table2 = process.doZhangSuenThinning(table, false);
            Bitmap elo2;
            elo2 = process.TableToBitmap(table2);
            elo2.Save("C:\\Users\\BuBu\\Desktop\\02.png");
        }
    }
}
