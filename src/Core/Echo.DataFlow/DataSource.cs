using System;

namespace Echo.DataFlow
{
    /// <summary>
    /// Represents a data source in a data flow graph.
    /// </summary>
    /// <typeparam name="TContents">The type of data stored in each data flow node.</typeparam>
    public abstract class DataSource<TContents>
    {
        /// <summary>
        /// Creates a new data source.
        /// </summary>
        /// <param name="node">The node producing the data.</param>
        protected DataSource(DataFlowNode<TContents> node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }
        
        /// <summary>
        /// Gets the data flow node that produced the data.
        /// </summary>
        public DataFlowNode<TContents> Node
        {
            get;
        }

        /// <summary>
        /// Determines whether the data sources are considered equal.
        /// </summary>
        /// <param name="other">The other data source.</param>
        protected virtual bool Equals(DataSource<TContents> other)
        {
            return Equals(Node, other.Node);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            return obj.GetType() == GetType() && Equals((DataSource<TContents>) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => Node.GetHashCode();
        
        /// <summary>
        /// Determines whether the data sources are considered equal.
        /// </summary>
        /// <param name="a">The first data source.</param>
        /// <param name="b">The second data source.</param>
        /// <returns><c>true</c> if they are considered equal, <c>false</c> otherwise.</returns>
        public static bool operator ==(DataSource<TContents> a, DataSource<TContents> b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether the data sources are not considered equal.
        /// </summary>
        /// <param name="a">The first data source.</param>
        /// <param name="b">The second data source.</param>
        /// <returns><c>true</c> if they are not considered equal, <c>false</c> otherwise.</returns>
        public static bool operator !=(DataSource<TContents> a, DataSource<TContents> b)
        {
            return !(a == b);
        }
        
    }
}