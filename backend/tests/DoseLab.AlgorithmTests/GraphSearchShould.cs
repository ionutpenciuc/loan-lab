using DoseLab.Algorithms;
using Xunit;

namespace DoseLab.AlgorithmTests;

public class GraphSearchShould
{
    [Fact]
    public void FindTheShortestPathInAnUnweightedGraph()
    {
        // setup

        var graph = new Graph(4);
        graph.AddEdge(0, 1);
        graph.AddEdge(0, 2);
        graph.AddEdge(1, 3);
        graph.AddEdge(2, 3);

        // execute

        var path = GraphSearch.ShortestPath(graph, 0, 3);

        // verify

        Assert.Equal([0, 1, 3], path);
    }

    [Fact]
    public void ReturnAnEmptyPathWhenTheGoalCannotBeReached()
    {
        // setup

        var graph = new Graph(3);
        graph.AddEdge(0, 1);

        // execute

        var path = GraphSearch.ShortestPath(graph, 0, 2);

        // verify

        Assert.Empty(path);
    }

    [Fact]
    public void ReportReachabilityWithDepthFirstSearch()
    {
        // setup

        var graph = new Graph(3);
        graph.AddEdge(0, 1);
        graph.AddEdge(1, 2);

        // execute

        var forward = GraphSearch.CanReach(graph, 0, 2);
        var backward = GraphSearch.CanReach(graph, 2, 0);

        // verify

        Assert.True(forward);
        Assert.False(backward);
    }
}
