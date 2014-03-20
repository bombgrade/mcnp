using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataStructures;

namespace NumberPartitioning
{
    public class MCGraph : Graph<MCNodeData>
    {
        public MCGraph() { }

        public GraphNode<MCNodeData> FindByID(int id)
        {
            foreach (GraphNode<MCNodeData> g in this.Nodes)
                if (g.Value.ID == id)
                    return g;

            return null;
        }
    }
}
