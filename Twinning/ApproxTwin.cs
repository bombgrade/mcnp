using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NumberPartitioning.Twinning
{
    public class ApproxTwin
    {
        List<Point> _points = new List<Point>();

        public ApproxTwin(double[,] data)
        {
            if (data.GetLength(1) > 1)
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    double[] p = new double[data.GetLength(1)];
                    for (int j = 0; j < data.GetLength(1); j++)
                        p[j] = data[i, j];

                    _points.Add(new Point(p));
                }
            }
            else
            {
                _data = new double[data.GetLength(0)];
                for (int i = 0; i < data.GetLength(0); i++)
                    _data[i] = data[i, 0];
            }
        }

        double[] _data;
        public ApproxTwin(double[] data)
        {
            _data = new double[data.GetLength(0)];
            data.CopyTo(_data, 0);
        }

        public double[][][] Partition()
        {
            if (_data != null)
            {
                _data = _data.OrderByDescending(d => d).ToArray();

                List<double[]> p1 = new List<double[]>();
                List<double[]> p2 = new List<double[]>();

                bool first = true;
                for (int i = 0; i < _data.GetLength(0); i += 2)
                {
                    p1.Add(new double[] { _data[((first) ? i : i + 1)] });
                    p2.Add(new double[] { _data[((first) ? i + 1 : i)] });
                    first = !first;
                }

                if (_data.GetLength(0) == p1.Count + p2.Count + 1)
                    p1.Add(new double[] { _data[_data.GetLength(0) - 1] });

                double[][][] r = new double[2][][];
                r[0] = p1.ToArray();
                r[1] = p2.ToArray();

                return r;
            }

            List<double[]> part1 = new List<double[]>();
            List<double[]> part2 = new List<double[]>();

            ClosestPair cp = new ClosestPair();
            cp.Points.AddRange(_points);

            bool one = true;
            while (cp.Points.Count > 1)
            {
                Segment split = cp.TargetedSearch();

                Segment comp1 = new Segment(split.P1, Point.Center(split.P1.Dimension));
                Segment comp2 = new Segment(split.P2, Point.Center(split.P1.Dimension));

                Point max;
                Point min;
                if (comp1.LengthSquared() > comp2.LengthSquared())
                {
                    max = split.P1;
                    min = split.P2;
                }
                else
                {
                    max = split.P2;
                    min = split.P1;
                }

                if (one)
                {
                    part1.Add(max.Coordinates);
                    part2.Add(min.Coordinates);
                }
                else
                {
                    part1.Add(min.Coordinates);
                    part2.Add(max.Coordinates);
                }

                one = !one;
                
                cp.Points.Remove(split.P1);
                cp.Points.Remove(split.P2);
            }

            if (cp.Points.Count == 1)
                part1.Add(cp.Points[0].Coordinates);

            double[][][] part = new double[2][][];
            part[0] = part1.ToArray();
            part[1] = part2.ToArray();

            return part;
        }
    }
}
