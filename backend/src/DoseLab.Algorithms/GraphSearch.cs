namespace DoseLab.Algorithms;

/// <summary>
/// Graph walks.
/// Breadth-first search finds the shortest path when every edge has the same cost.
/// Depth-first search reports whether a node can be reached.
/// </summary>
public static class GraphSearch
{
    /// <summary>
    /// Shortest path from <paramref name="start"/> to <paramref name="goal"/>, including both ends.
    /// Empty when no path exists. Time O(nodes + edges).
    /// </summary>
    public static IReadOnlyList<int> ShortestPath(Graph graph, int start, int goal)
    {
        ArgumentNullException.ThrowIfNull(graph);
        EnsureNode(graph, start);
        EnsureNode(graph, goal);

        if (start == goal)
            return [start];

        var previous = new int[graph.NodeCount];
        Array.Fill(previous, -1);
        var seen = new bool[graph.NodeCount];
        var queue = new Queue<int>();

        queue.Enqueue(start);
        seen[start] = true;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            foreach (var next in graph.Neighbors(node))
            {
                if (seen[next])
                    continue;

                seen[next] = true;
                previous[next] = node;

                if (next == goal)
                    return Rebuild(previous, goal);

                queue.Enqueue(next);
            }
        }

        return [];
    }

    /// <summary>
    /// True when a directed path from <paramref name="start"/> to <paramref name="goal"/> exists.
    /// </summary>
    public static bool CanReach(Graph graph, int start, int goal)
    {
        ArgumentNullException.ThrowIfNull(graph);
        EnsureNode(graph, start);
        EnsureNode(graph, goal);

        var seen = new bool[graph.NodeCount];
        return Visit(start);

        bool Visit(int node)
        {
            if (node == goal)
                return true;

            if (seen[node])
                return false;

            seen[node] = true;

            foreach (var next in graph.Neighbors(node))
            {
                if (Visit(next))
                    return true;
            }

            return false;
        }
    }

    private static int[] Rebuild(int[] previous, int goal)
    {
        var path = new List<int>();
        for (var node = goal; node != -1; node = previous[node])
            path.Add(node);

        path.Reverse();
        return path.ToArray();
    }

    private static void EnsureNode(Graph graph, int node)
    {
        if ((uint)node >= (uint)graph.NodeCount)
            throw new ArgumentOutOfRangeException(nameof(node));
    }
}
