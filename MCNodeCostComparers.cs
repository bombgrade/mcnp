using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberPartitioning
{
    public class MCNodeAscCostComparer : IComparer<MCNodeData>
    {
        #region IComparer<MCNodeData> Members

        public int Compare(MCNodeData x, MCNodeData y)
        {
            double xc = x.NodeCost;
            double yc = y.NodeCost;

            if (xc > yc)
                return 1;
            if (xc < yc)
                return -1;

            return 0;
        }

        #endregion
    }

    public class MCNodeDescCostComparer : IComparer<MCNodeData>
    {
        #region IComparer<MCNodeData> Members

        public int Compare(MCNodeData x, MCNodeData y)
        {
            MCNodeAscCostComparer asc = new MCNodeAscCostComparer();
            return -asc.Compare(x, y);
        }

        #endregion
    }
}
