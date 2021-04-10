using System.IO;
using System.Linq;
using Echo.DataFlow.Analysis;
using Echo.Platforms.DummyPlatform;
using Xunit;

namespace Echo.DataFlow.Tests.Analysis
{
    public class DependencyExplorerTest
    {

        [Fact]
        public void OneStackDependencyMultipleDataSources()
        {
            var dfg = new DataFlowGraph<int>(IntArchitecture.Instance);
            var n0 = dfg.Nodes.Add(0, 0);
            var n1 = dfg.Nodes.Add(1, 1);
            var n2 = dfg.Nodes.Add(2, 2);
            var n3 = dfg.Nodes.Add(3, 3);
            var n4 = dfg.Nodes.Add(4, 4);
            var n5 = dfg.Nodes.Add(5, 5);
            var n6 = dfg.Nodes.Add(6, 6);

            n0.StackDependencies.SetCount(2);
            n0.StackDependencies[0].Add(new StackDataSource<int>(n1));
            n0.StackDependencies[0].Add(new StackDataSource<int>(n2));
            n0.StackDependencies[1].Add(new StackDataSource<int>(n3));
            n0.StackDependencies[1].Add(new StackDataSource<int>(n4));

            n2.StackDependencies.SetCount(1);
            n2.StackDependencies[0].Add(new StackDataSource<int>(n5));
            n2.StackDependencies[0].Add(new StackDataSource<int>(n6));

            using (var fs = File.CreateText("/tmp/dfg.dot"))
                dfg.ToDotGraph(fs);
            
            var explorer = new DependencyExplorer<int>(DependencyCollectionFlags.IncludeStackDependencies);

            var allPaths = explorer.GetAllPossibleDependencies(n0).ToList();

        }
    }
}