using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataStructures;

namespace NumberPartitioning
{
    public class MCKK
    {
        public enum ArcType
        {
            Same = 0,
            Opposite = 1
        }

        private MCGraph _graph = new MCGraph();


        public MCKK(MCNodeData[] nodes)
        {
            SetNodes(nodes);
        }

        // alternative base constructor for derived classes
        protected MCKK() { }
        
        protected void SetNodes(MCNodeData[] nodes)
        {
            foreach (MCNodeData d in nodes)
                _graph.AddNode(d);

            initializePartitions();
        }

        public int N { get { return this._graph.Count; } }
        public List<MCNodeData>[] Subsets { get; protected set; }
        public MCNodeData Remainder { get; protected set; }
        public MCGraph Graph { get { return this._graph; } }

        public virtual double Partition()
        {
            //// define active set
            List<MCNodeData> l = this._graph.Nodes.Select(d => d.Value).ToList();
            l.Sort(new MCNodeDescCostComparer());

            // continue while active set as more than one node
            // final node is the remainder with the objective value
            while (l.Count > 1)
            {
                // initially take highest cost
                MCNodeData u = l[0];
                l.RemoveAt(0);

                // remainder node, to replace origin/larger
                MCNodeData rem = new MCNodeData();

                // index of wstar to track which to remove
                int wstarIndex = 0;

                // minimum net cost available
                // value will be set on initial iteration when 
                // calculated from first available node i.e. second
                // node available in the active set
                double minNet = double.MaxValue; //u.NodeCost;

                // only consider "next node" for single criteria
                // this is the standard NP problem
                int stopCount = (l[0].K == 1) ? 1 : l.Count;

                bool opposite = false;

                // check remaining nodes for wstar
                // stop looking if c(w) <= c(u) - c(uwc)
                for (int j = 0; j < stopCount; j++)
                {
                    MCNodeData w = l[j];

                    //if (w.NodeCost > u.NodeCost - l[wstarIndex].NodeCost || j == l.Count - 2)
                    //{
                    MCNodeData uwo = u - w;
                    MCNodeData uws = u + w;

                    if (uwo.NodeCost <= minNet || uws.NodeCost <= minNet)
                    {
                        opposite = uwo.NodeCost <= uws.NodeCost;
                        if (opposite)
                        {
                            rem = uwo;
                        }
                        else
                        {
                            rem = uws;
                        }

                        minNet = rem.NodeCost;
                        wstarIndex = j;
                    }
                }
                
                GraphNode<MCNodeData> gu = _graph.FindByID(u.ID);
                GraphNode<MCNodeData> gw = _graph.FindByID(l[wstarIndex].ID);
                //Console.WriteLine("Create arc " + gu.Value.ID + " " + gu.Value.NodeCost + " "
                //    + (int)((opposite) ? ArcType.Opposite : ArcType.Same) + " " + gw.Value.ID + " " + gw.Value.NodeCost);
                _graph.AddUndirectedEdge(gu, gw, (int)((opposite) ? ArcType.Opposite : ArcType.Same));

                l.RemoveAt(wstarIndex);

                // add nonzero cost nodes back to active set
                if (rem.NodeCost > 0)
                {
                    int addIndex = 0;
                    // find place to add remainder node
                    for (int j = l.Count - 1; j >= 0; j--)
                    {
                        if (rem.NodeCost <= l[j].NodeCost)
                        {
                            addIndex = j + 1;   // if index = count, then capacity auto adjusts
                            j = -1;
                        }
                    }

                    rem.ID = u.ID;
                    l.Insert(addIndex, rem);
                }
            }

            if (l.Count == 1)
                this.Remainder = l[0];
            else
            {
                this.Remainder = new MCNodeData(_graph.Nodes[0].Value.K);
            }

            return this.Remainder.NodeCost;
        }

        #region build subsets
        private NodeList<MCNodeData> _repop;
        public virtual void FillSubsets()
        {
            // storage for partitioned nodes
            initializePartitions();
            _repop = new NodeList<MCNodeData>();
            NodeList<MCNodeData> all = new NodeList<MCNodeData>(_graph.Nodes);            
            while (all.Count > 0)
            {
                GraphNode<MCNodeData> gn = (GraphNode<MCNodeData>)all[0];
                Subsets[0].Add(gn.Value);
                //all.Remove(gn);
                _repop.Add(all[0]);
                all.RemoveAt(0);
                all = splitAllChild(all, gn, 0);
            }
            _graph.Nodes.Clear();
            foreach (var a in _repop)
                _graph.Nodes.Add(a);
        }
        
        private NodeList<MCNodeData> splitAllChild(NodeList<MCNodeData> rem, GraphNode<MCNodeData> src, int loc)
        {
            for (int i = 0; i < src.Neighbors.Count; i++)
            {
                int locDest = src.Costs[i] == 1 ? 1 - loc : loc;
                if (!Subsets[0].Contains(src.Neighbors[i].Value) && !Subsets[1].Contains(src.Neighbors[i].Value))
                {
                    Subsets[locDest].Add(src.Neighbors[i].Value);
                    _repop.Add(src.Neighbors[i]);
                    rem.Remove((GraphNode<MCNodeData>)src.Neighbors[i]);
                    if (((GraphNode<MCNodeData>)src.Neighbors[i]).Neighbors.Count > 1)
                        rem = splitAllChild(rem, (GraphNode<MCNodeData>)src.Neighbors[i], locDest);
                }
                else
                {
                    _repop.Add(src.Neighbors[i]);
                    rem.Remove((GraphNode<MCNodeData>)src.Neighbors[i]);
                }
            }
            
            return rem;
        }
        #endregion

        private void initializePartitions()
        {
            this.Subsets = new List<MCNodeData>[2];
            this.Subsets[0] = new List<MCNodeData>();
            this.Subsets[1] = new List<MCNodeData>();
        }
    }
}
