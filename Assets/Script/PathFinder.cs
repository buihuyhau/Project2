using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public static PathFinder Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public LayerMask SolidLayer;

    /// <summary>
    /// Calculates the path from the PathFinder's position to the target position using the A* algorithm.
    /// </summary>
    /// <param name="target">The target position to navigate to.</param>
    /// <returns>A PathResult containing the path and any free nodes discovered.</returns>
    public PathResult GetPath(Vector2 target)
    {
        var pathToTarget = new List<Vector2>();
        var checkedNodes = new List<Node>();
        var waitingNodes = new List<Node>();

        Vector2 startPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        Vector2 targetPosition = new Vector2(Mathf.Round(target.x), Mathf.Round(target.y));

        if (startPosition == targetPosition)
            return new PathResult(pathToTarget, checkedNodes);

        Node startNode = new Node(0, startPosition, targetPosition, null);
        checkedNodes.Add(startNode);
        waitingNodes.AddRange(GetNeighbourNodes(startNode));

        while (waitingNodes.Count > 0)
        {
            Node nodeToCheck = waitingNodes.OrderBy(x => x.F).FirstOrDefault();

            if (nodeToCheck.Position == targetPosition)
            {
                pathToTarget = CalculatePathFromNode(nodeToCheck);
                return new PathResult(pathToTarget, checkedNodes);
            }

            bool walkable = !Physics2D.OverlapCircle(nodeToCheck.Position, 0.1f, SolidLayer);
            waitingNodes.Remove(nodeToCheck);
            if (!walkable)
            {
                checkedNodes.Add(nodeToCheck);
            }
            else
            {
                if (!checkedNodes.Any(x => x.Position == nodeToCheck.Position))
                {
                    checkedNodes.Add(nodeToCheck);
                    waitingNodes.AddRange(GetNeighbourNodes(nodeToCheck));
                }
            }
        }

        // Path not found; return empty path with checked nodes
        return new PathResult(pathToTarget, checkedNodes);
    }

    /// <summary>
    /// Reconstructs the path from the target node back to the start node.
    /// </summary>
    /// <param name="node">The target node.</param>
    /// <returns>A list of Vector2 positions representing the path.</returns>
    private List<Vector2> CalculatePathFromNode(Node node)
    {
        var path = new List<Vector2>();
        Node currentNode = node;

        while (currentNode.PreviousNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.PreviousNode;
        }

        path.Reverse();
        return path;
    }

    /// <summary>
    /// Generates neighbor nodes (up, down, left, right) for the given node.
    /// </summary>
    /// <param name="node">The current node.</param>
    /// <returns>A list of neighboring nodes.</returns>
    private List<Node> GetNeighbourNodes(Node node)
    {
        var neighbours = new List<Node>();

        Vector2[] directions = {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        foreach (var dir in directions)
        {
            Vector2 newPos = node.Position + dir;
            neighbours.Add(new Node(node.G + 1, newPos, node.TargetPosition, node));
        }

        return neighbours;
    }

    void OnDrawGizmos()
    {
        // Optional: Implement visualization if needed
    }
}

public class Node
{
    public Vector2 Position;
    public Vector2 TargetPosition;
    public Node PreviousNode;
    public int F; // F = G + H
    public int G; // Cost from start to this node
    public int H; // Heuristic cost to target

    public Node(int g, Vector2 position, Vector2 targetPosition, Node previousNode)
    {
        Position = position;
        TargetPosition = targetPosition;
        PreviousNode = previousNode;
        G = g;
        H = (int)(Mathf.Abs(TargetPosition.x - Position.x) + Mathf.Abs(TargetPosition.y - Position.y));
        F = G + H;
    }
}

public class PathResult
{
    /// <summary>
    /// The calculated path as a list of positions.
    /// </summary>
    public List<Vector2> Path { get; private set; }

    /// <summary>
    /// The list of nodes that were checked during pathfinding.
    /// </summary>
    public List<Node> FreeNodes { get; private set; }

    public PathResult(List<Vector2> path, List<Node> freeNodes)
    {
        Path = path;
        FreeNodes = freeNodes;
    }
}
