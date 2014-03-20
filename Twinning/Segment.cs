using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberPartitioning.Twinning
{
    public class Segment
    {
        public static double Length(Point p1, Point p2)
        {
            return Math.Sqrt(Segment.LengthSquared(p1, p2));
        }

        public static double LengthSquared(Point p1, Point p2)
        {
            if (p1.Dimension != p2.Dimension)
                throw new ArgumentException("Points must be of the same dimension");

            double accum = 0;
            for (int i = 0; i < p1.Coordinates.GetLength(0); i++)
                accum += (p1.Coordinates[i] - p2.Coordinates[i]) * (p1.Coordinates[i] - p2.Coordinates[i]);

            return accum;
        }

        public Segment(Point p1, Point p2)
        {
            if (p1.Dimension == p2.Dimension)
            {
                P1 = p1;
                P2 = p2;
            }
        }

        public readonly Point P1;
        public readonly Point P2;

        public double Length()
        {
            return Math.Sqrt(LengthSquared());
        }

        public double LengthSquared()
        {
            return Segment.LengthSquared(P1, P2);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.Append("P1:");
            StringBuilder sb2 = new StringBuilder("P2:");

            for (int i = 0; i < P1.Dimension; i++)
            {
                sb.Append(" " + P1.Coordinates[i]);
                sb2.Append(" " + P2.Coordinates[i]);
            }

            sb.AppendLine();
            sb.Append(sb2.ToString());

            return sb.ToString();
        }
    }
}
