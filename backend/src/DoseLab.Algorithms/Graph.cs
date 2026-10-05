namespace DoseLab.Algorithms;

/// <summary>
/// Directed graph. Node ids are 0, 1, 2, and so on.
/// </summary>
public sealed class Graph
{
    private readonly List<int>[] _edges;

    public Graph(int nodeCount)
    {
        if (nodeCount < 1)
            throw new ArgumentOutOfRangeException(nameof(nodeCount));

        _edges = new List<int>[nodeCount];
        for (var node = 0; node < nodeCount; node++)
            _edges[node] = [];
    }

    public int NodeCount => _edges.Length;

    public void AddEdge(int from, int to)
    {
        EnsureNode(from);
        EnsureNode(to);
        _edges[from].Add(to);
    }

    public IReadOnlyList<int> Neighbors(int node)
    {
        EnsureNode(node);
        return _edges[node];
    }

    private void EnsureNode(int node)
    {
        if ((uint)node >= (uint)_edges.Length)
            throw new ArgumentOutOfRangeException(nameof(node));
    }
}
