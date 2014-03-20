using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberPartitioning
{
    public class MCNPDistMatch : MCKK
    {
        private double[][] _data;
        private Dictionary<int, double[]> _dataID;

        private Func<double, int, double>[] _critFunc;
        private double[][] _scaling;
        private double[][] _moments;

        private int _n;
        private int _m;

        protected MCNPDistMatch() { }

        public MCNPDistMatch(double[,] nmData, int k)
            : base()
        {
            _n = nmData.GetLength(0);
            _m = nmData.GetLength(1);
            this.K = k;

            UseScaling = true;
            EqualCardinality = false;

            //double[] critavg = new double[_m];
            _critFunc = new Func<double, int, double>[_m];
            
            _data = new double[_m][];
            for (int i = 0; i < _m; i++)
            {
                double critAvg = 0;
                _data[i] = new double[_n];
                for (int j = 0; j < _n; j++)
                {
                    _data[i][j] = nmData[j, i];
                    critAvg += nmData[j, i] / _n;
                }
                
                _critFunc[i] = new Func<double, int, double>(
                    (x, n) => Math.Sign(x - critAvg) * Math.Pow(Math.Abs(x - critAvg), n));
            }
        }

        public MCNPDistMatch(double[] nmData, int k)
            : base()
        {
            _n = nmData.GetLength(0);
            _m = 1;
            this.K = k;

            UseScaling = false;
            EqualCardinality = false;

            //double[] critavg = new double[_m];
            _critFunc = new Func<double, int, double>[_m];

            _data = new double[_m][];
            for (int i = 0; i < _m; i++)
            {
                double critAvg = 0;
                _data[i] = new double[_n];
                for (int j = 0; j < _n; j++)
                {
                    _data[i][j] = nmData[j];
                    critAvg += nmData[j] / _n;
                }
                //Console.WriteLine("New " + critAvg);
                _critFunc[i] = new Func<double, int, double>(
                    (x, n) => Math.Sign(x - critAvg) * Math.Pow(Math.Abs(x - critAvg), n));
            }
        }

        public int K { get; protected set; }
        public bool UseScaling { get; set; }
        public bool EqualCardinality { get; set; }
        public double[][][] DatasetPartition { get; protected set; }

        public Dictionary<int, double[]> DataNodeMap()
        {
            return new Dictionary<int, double[]>(_dataID);
        }

        public MCNodeData[] BuildNodes()
        {
            this.setScale();

            this._dataID = new Dictionary<int, double[]>();

            MCNodeData[] nodes = new MCNodeData[_n];
            int dim = _m * this.K + ((this.EqualCardinality) ? 1 : 0);
            for (int i = 0; i < _n; i++)
            {
                nodes[i] = new MCNodeData(dim);

                double[] row = new double[_m];
                for (int j = 0; j < _m; j++)
                {
                    for (int sc = 0; sc < this.K; sc++)
                    {
                        nodes[i][j * this.K + sc] = _critFunc[j](_data[j][i], sc + 1) * _scaling[j][sc];
                    }

                    row[j] = _data[j][i];
                }

                if (this.EqualCardinality)
                {
                    nodes[i][dim - 1] = double.MaxValue/2;
                }

                nodes[i].ID = MCNodeData.GetNewID();
                this._dataID.Add(nodes[i].ID, row);
            }

            this.SetNodes(nodes);

            return nodes;
        }

        public override void FillSubsets()
        {
            base.FillSubsets();
            this.convertPartition();
        }

        #region private methods
        private void setMoments()
        {
            _moments = new double[_m][];
            for (int i = 0; i < _m; i++)
                _moments[i] = firstKMoment(_data[i], K);
        }

        private void setScale()
        {
            this._scaling = new double[_m][];
            if (this.UseScaling)
            {
                if (this._moments == null)
                    this.setMoments();

                for (int j = 0; j < _m; j++)
                {
                    this._scaling[j] = new double[this.K];
                    for (int sc = 0; sc < this.K; sc++)                    
                        this._scaling[j][sc] = 1.0 / this._moments[j][sc];
                }
            }
            else
            {
                for (int j = 0; j < _m; j++)
                {
                    this._scaling[j] = new double[this.K];
                    for (int sc = 0; sc < this.K; sc++)
                        this._scaling[j][sc] = 1.0;
                }
            }
        }

        private double[] firstKMoment(double[] data, int k)
        {
            int n = data.GetLength(0);
            double[] res = new double[k];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < k; j++)
                    res[j] += Math.Pow(data[i], j+1) / n;
            }

            return res;
        }

        private void convertPartition()
        {
            double[][][] dataPart = new double[2][][];

            for (int i = 0; i < 2; i++)
            {
                dataPart[i] = new double[this.Subsets[i].Count][];
                for (int j = 0; j < dataPart[i].GetLength(0); j++)
                {
                    if (this._dataID.ContainsKey(this.Subsets[i][j].ID))
                        dataPart[i][j] = this._dataID[this.Subsets[i][j].ID];
                }
            }

            this.DatasetPartition = dataPart;
        }
        #endregion
    }
}
