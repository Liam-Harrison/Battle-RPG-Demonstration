using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public struct Connection
{
    public float weight;
    public float height;
    public NavTile tile;
}

public class NavTile : IHeapItem<NavTile>
{
    public TilePosition tile;
    public List<Connection> connections;
    public Transform enviormentObject;
    public bool walkable;
    public float gCost;
    public float hCost;
    public NavTile parent;
    int heapIndex;
    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex { get => heapIndex; set => heapIndex = value; }

    public int CompareTo(NavTile other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}

public delegate void callback(PathResult results);

public struct PathRequest
{
    public TilePosition start;
    public TilePosition end;
    public callback callback;
    public MoveRules rules;

    public PathRequest(TilePosition _start, TilePosition _end, MoveRules _rules, callback _callback)
    {
        start = _start;
        end = _end;
        callback = _callback;
        rules = _rules;
    }
}

public struct PathResult
{
    public bool succeeded;
    public TilePosition start;
    public TilePosition target;
    public NavTile[] path;
    public long elaspedMilliseconds;
    public float cost;

    public PathResult(bool _passed = false) : this(false, new TilePosition(), new TilePosition(), new NavTile[] { }, 0f, 0L)
    {

    }

    public PathResult(bool _passed, TilePosition _start, TilePosition _target, NavTile[] _path, float _cost, long _time)
    {
        succeeded = _passed;
        start = _start;
        target = _target;
        path = _path;
        elaspedMilliseconds = _time;
        cost = _cost;
    }
}

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;
    public static LevelManager Instance { get => instance; }

    private static List<NavTile> tiles = new List<NavTile>();

    public static void RequestPath(PathRequest request)
    {
        IEnumerator routine = Pathfinding.ComputePath(request);
        instance.StartCoroutine(routine);
    }

    public static int GetNavMeshCount
    {
        get
        {
            return tiles.Count;
        }
    }

    public static bool GetExistingNavTile(TilePosition tile, out NavTile nav, float heightCutoff = 0f)
    {
        foreach (var t in tiles)
        {
            if (t.tile.X == tile.X && t.tile.Z == tile.Z && GetAbsDistanceBetweenTiles(t.tile, tile) <= heightCutoff)
            {
                nav = t;
                return true;
            }
        }
        nav = new NavTile { tile = new TilePosition(), connections = null };
        return false;
    }

    public static List<NavTile> GetAdjacentNavTiles(NavTile tile, float heightCutoff)
    {
        List<NavTile> list = new List<NavTile>();
        var temp = tile.tile;
        NavTile t;
        temp.X = temp.X + 1;
        if (GetExistingNavTile(temp, out t, heightCutoff))
        {
            list.Add(t);
        }
        temp.X = temp.X - 2;
        if (GetExistingNavTile(temp, out t, heightCutoff))
        {
            list.Add(t);
        }
        temp.X = temp.X + 1;
        temp.Z = temp.Z + 1;
        if (GetExistingNavTile(temp, out t, heightCutoff))
        {
            list.Add(t);
        }
        temp.Z = temp.Z - 2;
        if (GetExistingNavTile(temp, out t, heightCutoff))
        {
            list.Add(t);
        }
        return list;
    }

    public static float GetDistanceBetweenTiles(NavTile a, NavTile b, bool YOnly = true)
    {
        if (YOnly) return a.tile.WorldY - b.tile.WorldY;
        return a.tile.WorldX - b.tile.WorldX + a.tile.WorldY - b.tile.WorldY + a.tile.WorldZ - b.tile.WorldZ;
    }

    public static float GetAbsDistanceBetweenTiles(NavTile a, NavTile b, bool YOnly = true)
    {
        if (YOnly) return Mathf.Abs(a.tile.WorldY - b.tile.WorldY);
        return Mathf.Abs(a.tile.WorldX - b.tile.WorldX) + Mathf.Abs(a.tile.WorldY - b.tile.WorldY) + Mathf.Abs(a.tile.WorldZ - b.tile.WorldZ);
    }

    public static float GetAbsDistanceBetweenTiles(TilePosition a, TilePosition b, bool YOnly = true)
    {
        if (YOnly) return Mathf.Abs(a.WorldY - b.WorldY);
        return Mathf.Abs(a.WorldX - b.WorldX) + Mathf.Abs(a.WorldY - b.WorldY) + Mathf.Abs(a.WorldZ - b.WorldZ);
    }

    protected static int AttatchToExistingNeighbours(NavTile tile, float heightCutoff = 2f)
    {
        int counter = 0;
        foreach (var i in GetAdjacentNavTiles(tile, heightCutoff))
        {
            var weight = Mathf.Clamp(GetAbsDistanceBetweenTiles(tile, i, true) - 0.5f, 0, Mathf.Infinity) + 1;
            tile.connections.Add(new Connection { tile = i, weight = weight, height = GetDistanceBetweenTiles(i, tile, true) });
            i.connections.Add(new Connection { tile = tile, weight = weight, height = GetDistanceBetweenTiles(tile, i, true) });
            counter++;
        }
        return counter;
    }

    protected static bool CreateNewNavTile(TilePosition tile, Transform enviormentObject = null)
    {
        if (GetExistingNavTile(tile, out NavTile nav))
        {
            UnityEngine.Debug.LogAssertion("NavTile already exists in this position (X: " + tile.X + ", Y: " + tile.Y + ", Z: " + tile.Z + ").\n" + UnityEngine.StackTraceUtility.ExtractStackTrace(), LevelManager.Instance);
            return false;
        }
        else
        {
            tiles.Add(new NavTile { tile = tile, connections = new List<Connection>(), enviormentObject = enviormentObject, walkable = true});
            return true;
        }
    }

    protected static bool CreateNewNavTile(TilePosition tile, out NavTile _nav, Transform enviormentObject = null)
    {
        _nav = new NavTile();
        if (GetExistingNavTile(tile, out NavTile nav))
        {
            UnityEngine.Debug.LogAssertion("NavTile already exists in this position (X: " + tile.X + ", Y: " + tile.Y + ", Z: " + tile.Z + ").\n" + UnityEngine.StackTraceUtility.ExtractStackTrace(), LevelManager.Instance);
            return false;
        }
        else
        {
            var n = new NavTile { tile = tile, connections = new List<Connection>(), enviormentObject = enviormentObject, walkable = true };
            _nav = n;
            tiles.Add(n);
            return true;
        }
    }

    protected static bool ExistsInNavMesh(TilePosition tile)
    {
        foreach (var t in tiles)
        {
            if (t.tile == tile) return true;
        }
        return false;
    }

    public static void GenerateNavMesh(bool debug = false)
    {
        int counter = 0, objCounter = 0, connectionCounter = 0;
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        List<Collider> colliders = new List<Collider>();

        colliders.AddRange(instance.GetComponentsInChildren<Collider>());
        // colliders.AddRange(instance.GetComponentsInChildren<MeshCollider>());

        foreach (var collider in colliders)
        {
            objCounter++;
            var bounds = collider.bounds;
            //var oldLayer = collider.gameObject.layer;
            //collider.gameObject.layer = 11;

            for (float x = Mathf.Floor(bounds.min.x) + Tile.tileWidth / 2 ; x <= bounds.max.x; x += Tile.tileWidth)
            {
                for (float z = Mathf.Floor(bounds.min.z) + Tile.tileWidth / 2; z <= bounds.max.z; z += Tile.tileWidth)
                {
                    var ray = new Ray(new Vector3(x, bounds.max.y + 1, z), Vector3.down);
                    if (Physics.Raycast(ray, out RaycastHit hit, bounds.max.y - bounds.min.y + 1, LayerMask.GetMask("Ground")))
                    {
                        var newPos = hit.point + new Vector3(0, 0.1f, 0);
                        if (hit.transform != collider.transform) continue;
                        var t = Tile.GetTile(newPos);
                        if (ExistsInNavMesh(t)) continue;
                        else
                        {
                            bool insideCollider = false;
                            foreach (var nCollider in instance.GetComponentsInChildren<MeshCollider>())
                            {
                                if (nCollider == collider) continue;
                                if (nCollider.bounds.Contains(t.MiddleBase))
                                {
                                    insideCollider = true;
                                    break;
                                }
                            }
                            if (!insideCollider)
                            {
                                CreateNewNavTile(t, out NavTile j, collider.transform);
                                connectionCounter += AttatchToExistingNeighbours(j, 2f);
                            }
                            counter++;
                        }
                    }
                }
            }

            //collider.gameObject.layer = oldLayer;
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log("Generated <color=red>" + tiles.Count + "</color> nodes with <color=red>" + connectionCounter + "</color> connections on <color=red>" + objCounter + "</color> enviormental objects in <color=red>" + stopwatch.ElapsedMilliseconds + "</color> milliseconds.", instance);
    }

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GenerateNavMesh();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (instance == null) return;
        foreach (var item in tiles)
        {
            foreach (var obj in UnityEditor.Selection.gameObjects)
            {
                if (obj.transform == item.enviormentObject)
                {
                    foreach (var i in item.connections)
                    {
                        if (i.weight <= 1f) Gizmos.color = Color.green;
                        else if (i.weight <= 2f) Gizmos.color = Color.yellow;
                        else Gizmos.color = Color.red;
                        Gizmos.DrawLine(item.tile.Middle, i.tile.tile.Middle);
                    }

                    Gizmos.color = Color.red;

                    Gizmos.DrawCube(item.tile.Middle, new Vector3(0.1f, 0.1f, 0.1f));
                }
            }
        }
    }
#endif

    void Update()
    {
        
    }
}
