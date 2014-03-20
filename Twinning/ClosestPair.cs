using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberPartitioning.Twinning
{
    public class ClosestPair
    {
        public ClosestPair() { }

        public readonly List<Point> Points = new List<Point>();

        public Segment DivideAndConquer()
        {
            if (Points[0].Dimension == 1)
                return oneDSort(Points);
            else
                return divConq(Points.OrderBy(p => p.Coordinates[0]).ToList());
        }

        private Segment divConq(List<Point> pointsByX)
        {
            int count = pointsByX.Count;
            if (count <= 4)
                return BruteForce(pointsByX);

            // left and right lists sorted by X, as order retained from full list
            var leftByX = pointsByX.Take(count / 2).ToList();
            var leftResult = divConq(leftByX);

            var rightByX = pointsByX.Skip(count / 2).ToList();
            var rightResult = divConq(rightByX);

            var result = rightResult.Length() < leftResult.Length() ? rightResult : leftResult;

            // There may be a shorter distance that crosses the divider
            // Thus, extract all the points within result.Length either side
            var midX = leftByX.Last().Coordinates[0];
            var bandWidth = result.Length();
            var inBandByX = pointsByX.Where(p => Math.Abs(midX - p.Coordinates[0]) <= bandWidth);

            // Sort by Y, so we can efficiently check for closer pairs
            var inBandByY = inBandByX.OrderBy(p => p.Coordinates[1]).ToArray();

            int iLast = inBandByY.Length - 1;
            for (int i = 0; i < iLast; i++)
            {
                var pLower = inBandByY[i];

                for (int j = i + 1; j <= iLast; j++)
                {
                    var pUpper = inBandByY[j];

                    // Comparing each point to successivly increasing Y values
                    // Thus, can terminate as soon as deltaY is greater than best result
                    if ((pUpper.Coordinates[1] - pLower.Coordinates[1]) >= result.Length())
                        break;

                    if (Segment.Length(pLower, pUpper) < result.Length())
                        result = new Segment(pLower, pUpper);
                }
            }

            return result;
        }

        public Segment BruteForce()
        {
            if (Points[0].Dimension == 1)
                return oneDSort(Points);
            else
                return BruteForce(Points);
        }
        
        private Segment BruteForce(List<Point> points)
        {
            //Trace.Assert(points.Count >= 2);

            int count = points.Count;

            // Seed the result - doesn't matter what points are used
            // This just avoids having to do null checks in the main loop below
            var result = new Segment(points[0], points[1]);
            var bestLength = result.Length();

            for (int i = 0; i < count; i++)
                for (int j = i + 1; j < count; j++)
                    if (Segment.Length(points[i], points[j]) < bestLength)
                    {
                        result = new Segment(points[i], points[j]);
                        bestLength = result.Length();
                    }

            return result;
        }

        public Segment TargetedSearch()
        {
            if (Points[0].Dimension == 1)
                return oneDSort(Points);
            else
                return TargetedSearch(Points);
        }

        private Segment TargetedSearch(List<Point> points)
        {
            //Trace.Assert(points.Count >= 2);

            int count = points.Count;
            points.Sort((lhs, rhs) => lhs.Coordinates[0].CompareTo(rhs.Coordinates[0]));

            var result = new Segment(points[0], points[1]);
            var bestLength = result.Length();

            for (int i = 0; i < count; i++)
            {
                var from = points[i];

                for (int j = i + 1; j < count; j++)
                {
                    var to = points[j];

                    var dx = to.Coordinates[0] - from.Coordinates[0];
                    if (dx >= bestLength)
                    {
                        break;
                    }

                    if (Segment.Length(from, to) < bestLength)
                    {
                        result = new Segment(from, to);
                        bestLength = result.Length();
                    }
                }
            }

            return result;
        }

        private Segment oneDSort(List<Point> points)
        {
            points.Sort((p1, p2) => p1.Coordinates[0].CompareTo(p2.Coordinates[0]));

            int ix = 0;
            double min = double.MaxValue;
            for (int i = 1; i < points.Count; i++)
            {
                double l = Math.Abs(points[i].Coordinates[0] - points[i - 1].Coordinates[0]);
                if (l < min)
                {
                    min = l;
                    ix = i - 1;
                }
            }

            return new Segment(points[ix], points[ix + 1]);
        }
    }
}
