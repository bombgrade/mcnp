using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberPartitioning.Twinning
{
    public class Point
    {
        public static Point Center(int dim)
        {
            return new Point(dim);
        }

        public Point(int dim)
        {
            Dimension = dim;
            if (dim > 0)
                Coordinates = new double[dim];
        }

        public Point(double[] coordinates)
        {
            if (coordinates != null)
            {
                Dimension = coordinates.GetLength(0);
                Coordinates = new double[Dimension];
                coordinates.CopyTo(Coordinates, 0);
            }
        }

        public readonly int Dimension;
        public readonly double[] Coordinates;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            foreach (double d in Coordinates)
                sb.Append(" " + d);
            return sb.ToString();
        }
    }
}
