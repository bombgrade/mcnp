using System;
using System.Text;

namespace NumberPartitioning
{
    public class MCNodeData
    {
        #region id management; not forced at creation
        private static NodeID _lockID = new NodeID();
        private class NodeID
        {
            private static int _lastID = 0;
            public int NewID() { return ++_lastID; }
        }

        public static int GetNewID()
        {
            int newID = 0;
            lock (_lockID)
            {
                newID = _lockID.NewID();
            }

            return newID;
        }
        #endregion

        private double[] _data;
        private double _nodeCost = 0;

        public MCNodeData() { SetDimension(1);  }
        public MCNodeData(int k) { SetDimension(k); }
        public MCNodeData(double[] d) 
        {
            if (d != null)
            {
                SetDimension(d.GetLength(0));
                for (int i = 0; i < K; i++)
                    this[i] = d[i];
            }
        }

        public int ID { get; set; }
        public int K { get { return _data.GetLength(0); } }
        public double NodeCost { get { return _nodeCost; } }

        public double this[int i]
        {
            get { return _data[i]; }
            set 
            {
                double old = Math.Abs(_data[i]);
                _data[i] = value;
                _nodeCost += Math.Abs(value) - old;
            }
        }

        public void SetDimension(int k)
        {
            _data = new double[k];
        }

        public override bool Equals(object obj)
        {
            if (obj is MCNodeData)
            {
                MCNodeData n2 = obj as MCNodeData;

                bool eq = ID == n2.ID && K == n2.K;
                if (eq)
                {
                    for (int i = 0; i < K && eq; i++)
                        eq = _data[i] == n2[i];
                }

                return eq;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder(this.GetType() + ": \n");
            s.Append("\tCost: " + this.NodeCost);
            s.Append("\tID: " + ID);
            if (K > 1)
            {
                s.Append("\n");
                for (int i = 0; i < K; i++)
                    s.Append("\t" + (i + 1) + ": " + this[i] + "\n");
            }

            return s.ToString();
        }

        public string ToString(bool horizontal)
        {
            if (!horizontal)
                return this.ToString();
            else
            {
                StringBuilder s = new StringBuilder(this.GetType() + ": ");
                s.Append("\tCost: " + this.NodeCost);
                s.Append("\tID: " + ID);
                if (this.K > 1)
                {
                    s.Append("\t");
                    for (int i = 0; i < this.K; i++)
                        s.Append((i + 1) + ": " + this[i] + "\t");
                }

                return s.ToString();
            }
        }

        #region add/subtract
        public static MCNodeData operator +(MCNodeData node1, MCNodeData node2)
        {
            if (node1.K == node2.K)
            {
                MCNodeData ret = new MCNodeData(node1.K);
                for (int i = 0; i < node1.K; i++)
                    ret[i] = node1[i] + node2[i];

                return ret;
            }
            else
                throw new ArgumentException("Node Data dimensions must agree");
        }

        public static MCNodeData operator -(MCNodeData node1, MCNodeData node2)
        {
            if (node1.K == node2.K)
            {
                MCNodeData ret = new MCNodeData(node1.K);
                for (int i = 0; i < node1.K; i++)
                    ret[i] = node1[i] - node2[i];

                return ret;
            }
            else
                throw new ArgumentException("Node Data dimensions must agree");
        }
        #endregion
    }
}
