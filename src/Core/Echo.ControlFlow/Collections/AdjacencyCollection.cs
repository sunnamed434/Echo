using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Echo.Core.Code;
using Echo.Core.Graphing;

namespace Echo.ControlFlow.Collections
{
    /// <summary>
    /// Represents a collection of edges originating from a single node.
    /// </summary>
    /// <typeparam name="TContents">The type of data that each node stores.</typeparam>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public class AdjacencyCollection<TContents> : ICollection<ControlFlowEdge<TContents>>
    {
        private readonly IDictionary<INode, ICollection<ControlFlowEdge<TContents>>> _neighbours 
            = new Dictionary<INode, ICollection<ControlFlowEdge<TContents>>>();

        private readonly ControlFlowEdgeType _edgeType;
        private int _count;
        
        internal AdjacencyCollection(ControlFlowNode<TContents> owner, ControlFlowEdgeType edgeType)
        {
            _edgeType = edgeType;
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        /// <inheritdoc />
        public int Count => _count;

        /// <inheritdoc />
        public bool IsReadOnly => false;
        
        /// <summary>
        /// Gets the node that all edges are originating from.
        /// </summary>
        public ControlFlowNode<TContents> Owner
        {
            get;
        }

        /// <summary>
        /// Creates and adds a edge to the provided node.
        /// </summary>
        /// <param name="neighbour">The new neighbouring node.</param>
        /// <returns>The created edge.</returns>
        public ControlFlowEdge<TContents> Add(ControlFlowNode<TContents> neighbour)
        {
            var edge = new ControlFlowEdge<TContents>(Owner, neighbour, _edgeType);
            Add(edge);
            return edge;
        }

        /// <summary>
        /// Adds an edge to the adjacency collection.
        /// </summary>
        /// <param name="edge">The edge to add.</param>
        /// <returns>The edge that was added.</returns>
        /// <exception cref="ArgumentException">
        /// Occurs when the provided edge cannot be added to this collection because of an invalid source node or edge type.
        /// </exception>
        public ControlFlowEdge<TContents> Add(ControlFlowEdge<TContents> edge)
        {
            AssertEdgeValidity(Owner, _edgeType, edge);
            GetEdges(edge.Target).Add(edge);
            edge.Target.IncomingEdges.Add(edge);
            _count++;
            return edge;
        }

        /// <inheritdoc />
        void ICollection<ControlFlowEdge<TContents>>.Add(ControlFlowEdge<TContents> edge)
        {
            Add(edge);
        }

        /// <inheritdoc />
        public void Clear()
        {
            foreach (var item in this.ToArray())
                Remove(item);
            _count = 0;
        }

        /// <summary>
        /// Determines whether a node is a neighbour of the current node. That is, determines whether there exists
        /// at least one edge between the current node and the provided node.
        /// </summary>
        /// <param name="neighbour">The node to check.</param>
        /// <returns><c>True</c> if the provided node is a neighbour, <c>false</c> otherwise.</returns>
        public bool Contains(ControlFlowNode<TContents> neighbour)
        {
            return GetEdges(neighbour).Count > 0;
        }

        /// <inheritdoc />
        public bool Contains(ControlFlowEdge<TContents> item)
        {
            return GetEdges(item.Target).Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(ControlFlowEdge<TContents>[] array, int arrayIndex)
        {
            foreach (var edges in _neighbours.Values)
            {
                edges.CopyTo(array, arrayIndex);
                arrayIndex += edges.Count;
            }
        }
        
        /// <summary>
        /// Removes all edges originating from the current node to the provided neighbour.
        /// </summary>
        /// <param name="neighbour">The neighbour to cut ties with.</param>
        /// <returns><c>True</c> if at least one edge was removed, <c>false</c> otherwise.</returns>
        public bool Remove(ControlFlowNode<TContents> neighbour)
        {
            var edges = GetEdges(neighbour);
            if (edges.Count > 0)
            {
                foreach (var edge in edges.ToArray())
                    Remove(edge);
                return true;
            }

            return false;
        }
        
        /// <inheritdoc />
        public bool Remove(ControlFlowEdge<TContents> edge)
        {
            bool result = GetEdges(edge.Target).Remove(edge);
            if (result)
            {
                _count--;
                edge.Target.IncomingEdges.Remove(edge);
            }

            return result;
        }
        
        /// <inheritdoc />
        public IEnumerator<ControlFlowEdge<TContents>> GetEnumerator()
        {
            return _neighbours
                .SelectMany(x => x.Value)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal static void AssertEdgeValidity(ControlFlowNode<TContents> owner, ControlFlowEdgeType type, ControlFlowEdge<TContents> item)
        {
            if (item.Type != type)
            {
                throw new ArgumentException(
                    $"Cannot add an edge of type {item.Type} to a collection of type {type}.");
            }
            
            if (item.Origin != owner)
                throw new ArgumentException("Cannot add an edge originating from a different node.");
        }

        private ICollection<ControlFlowEdge<TContents>> GetEdges(INode target)
        {
            if (!_neighbours.TryGetValue(target, out var edges))
            {
                edges = new HashSet<ControlFlowEdge<TContents>>();
                _neighbours[target] = edges;
            }

            return edges;
        }
        
    }
}