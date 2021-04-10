using System.Collections.Generic;
using System.Collections.Immutable;

namespace Echo.DataFlow.Analysis
{
    public class DependencyExplorer<T>
    {
        private readonly DependencyCollectionFlags _flags;

        public DependencyExplorer(DependencyCollectionFlags flags)
        {
            _flags = flags;
        }

        public IEnumerable<ImmutableList<DataFlowNode<T>>> GetAllPossibleDependencies(DataFlowNode<T> currentNode)
        {
            // Collection stage.
            var individualPaths = new List<List<ImmutableList<DataFlowNode<T>>>>();
            foreach (var dependency in currentNode.StackDependencies)
                individualPaths.Add(GetAllPossiblePaths(dependency));

            if (individualPaths.Count == 0)
                return new[] {ImmutableList.Create(currentNode)};
            
            // Combine stage
            var combinedPaths = individualPaths[0];
            for (int i = 1; i < individualPaths.Count; i++)
                combinedPaths = Combine(combinedPaths, individualPaths[i]);

            for (int i = 0; i < combinedPaths.Count; i++)
                combinedPaths[i] = combinedPaths[i].Add(currentNode);
            return combinedPaths;
        }

        public static List<ImmutableList<DataFlowNode<T>>> Combine(
            List<ImmutableList<DataFlowNode<T>>> a,
            List<ImmutableList<DataFlowNode<T>>> b)
        {
            var result = new List<ImmutableList<DataFlowNode<T>>>();

            foreach (var path1 in a)
            {
                foreach (var path2 in b)
                    result.Add(path1.AddRange(path2));
            }
            
            return result;
        }
        
        private List<ImmutableList<DataFlowNode<T>>> GetAllPossiblePaths(StackDependency<T> dependency)
        {
            var result = new List<ImmutableList<DataFlowNode<T>>>();
            foreach (var source in dependency)
                result.AddRange(GetAllPossibleDependencies(source.Node));
            return result;
        }

        
    }
}