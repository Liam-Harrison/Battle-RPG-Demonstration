using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{
    private static long computeTime;

    public static long ComputeTime
    {
        get
        {
            return computeTime;
        }
    }

    public static IEnumerator ComputePath(PathRequest request)
    {

        NavTile _start, _target;
        if (!LevelManager.GetExistingNavTile(request.start, out _start))
        {
            UnityEngine.Debug.LogAssertion("No NavTile could be found matching the passed TilePosition.\n" + UnityEngine.StackTraceUtility.ExtractStackTrace(), LevelManager.Instance);
            request.callback(new PathResult(false));
            yield break;
        }
        if (!LevelManager.GetExistingNavTile(request.end, out _target))
        {
            UnityEngine.Debug.LogAssertion("No NavTile could be found matching the passed TilePosition.\n" + UnityEngine.StackTraceUtility.ExtractStackTrace(), LevelManager.Instance);
            request.callback(new PathResult(false));
            yield break;
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Heap<NavTile> open = new Heap<NavTile>(LevelManager.GetNavMeshCount);
        HashSet<NavTile> closed = new HashSet<NavTile>();

        open.Add(_start);
        int iterationCountdown = 75;
        int iterations = iterationCountdown;

        while (open.Count > 0)
        {
            iterations--;

            var node = open.RemoveFirst();
            closed.Add(node);

            if (node.tile == request.end)
            {
                var path = ComputeFinalPath(_start, _target, out float cost);
                stopwatch.Stop();
                request.callback(new PathResult(true, request.start, request.end, path.ToArray(), cost, stopwatch.ElapsedMilliseconds));
                yield break;
            }

            if (iterations == 0)
            {
                iterations = iterationCountdown;
                yield return new WaitForFixedUpdate();
            }

            var neighbours = node.connections;
            for (int j = 0; j < neighbours.Count; j++)
            {
                var i = neighbours[j];
                if (!CanTraverseTo(i, request.rules) || closed.Contains(i.tile)) continue;

                float gCost = node.gCost + i.weight;
                if (gCost < i.tile.gCost || !open.Contains(i.tile))
                {
                    i.tile.gCost = gCost;
                    i.tile.hCost = LevelManager.GetAbsDistanceBetweenTiles(i.tile, _target, false);
                    i.tile.parent = node;

                    if (!open.Contains(i.tile))
                    {
                        open.Add(i.tile);
                    }
                    else
                    {
                        open.UpdateItem(i.tile);
                    }
                }
            }
        }

        request.callback(new PathResult(false));
        yield break;
    }

    private static bool CanTraverseTo(Connection connection, MoveRules rules)
    {
        if (!connection.tile.walkable) return false;
        if (connection.tile.tile.GetEntity() != null)
        {
            if (connection.tile.tile.GetEntity().rules.blocksMovement) return false;
        }
        if (connection.height > 0 && connection.height > rules.maxJump) return false;
        if (connection.height < 0 && connection.height > rules.maxDrop) return false;
        return true;
    }

    private static bool IsTargetAboveTargetTile(NavTile target, NavTile origin)
    {
        if (LevelManager.GetDistanceBetweenTiles(target, origin, true) > 0) return true;
        return false;
    }

    private static List<NavTile> ComputeFinalPath(NavTile start, NavTile end, out float cost)
    {
        List<NavTile> path = new List<NavTile>();
        var current = end;
        cost = 0;

        while (current != start)
        {
            path.Add(current);

            foreach (var i in current.connections)
            {
                if (i.tile == current.parent) cost += i.weight;
            }

            current = current.parent;
            
        }
        path.Reverse();
        return path;
    }
}
