using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektBjometria
{
    class Pixel
    {
        public Point point { get; set; }

        public Color color { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Pixel)
            {
                Pixel pi = (Pixel)obj;
                Point p = pi.point;
                return (p.X == this.point.X) && (p.Y == this.point.Y);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
