using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct TilePosition
{
    public TilePosition(int x, int y, int z)
    {
        this.x = 0;
        this.y = 0;
        this.z = 0;
        this.worldX = 0;
        this.worldY = 0;
        this.worldZ = 0;
        X = x;
        Y = y;
        Z = z;
    }

    public TilePosition(Vector3 vector) : this(vector.x, vector.y, vector.z)
    {
    }

    public TilePosition(float x, float y, float z)
    {
        this.x = 0;
        this.y = 0;
        this.z = 0;
        this.worldX = 0;
        this.worldY = 0;
        this.worldZ = 0;
        WorldX = x;
        WorldY = y;
        WorldZ = z;
    }

    private int x;
    private int y;
    private int z;
    private float worldX;
    private float worldY;
    private float worldZ;

    public int X
    {
        get => x;
        set
        {
            x = value;
            worldX = value * Tile.tileWidth;
        }
    }

    public int Y
    {
        get => y;
        set
        {
            y = value;
            worldY = value * Tile.tileHeight;
        }
    }

    public int Z
    {
        get => z;
        set
        {
            z = value;
            worldZ = value * Tile.tileWidth;
        }
    }

    public float WorldX
    {
        get => worldX;
        set
        {
            x = Mathf.FloorToInt(value / Tile.tileWidth);
            worldX = x * Tile.tileWidth;
        }
    }

    public float WorldY
    {
        get => worldY;
        set
        {
            y = Mathf.FloorToInt(value / Tile.tileHeight);
            worldY = y * Tile.tileHeight;
        }
    }

    public float WorldZ
    {
        get => worldZ;
        set
        {
            z = Mathf.FloorToInt(value / Tile.tileWidth);
            worldZ = z * Tile.tileWidth;
        }
    }

    public Vector3 Middle
    {
        get
        {
            return new Vector3(WorldX + WidthExtents, WorldY + HeightExtents, WorldZ + WidthExtents);
        }
    }

    public Vector3 MiddleBase
    {
        get
        {
            return new Vector3(WorldX + WidthExtents, WorldY, WorldZ + WidthExtents);
        }
    }

    public float WidthExtents
    {
        get
        {
            return Tile.tileWidth / 2;
        }
    }

    public float HeightExtents
    {
        get
        {
            return Tile.tileHeight / 2;
        }
    }

    public static bool operator ==(TilePosition a, TilePosition b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(TilePosition a, TilePosition b)
    {
        return !Equals(a, b);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is TilePosition))
        {
            return false;
        }

        var position = (TilePosition)obj;
        return X == position.X &&
               Y == position.Y &&
               Z == position.Z;
    }

    public override int GetHashCode()
    {
        var hashCode = -307843816;
        hashCode = hashCode * -1521134295 + X.GetHashCode();
        hashCode = hashCode * -1521134295 + Y.GetHashCode();
        hashCode = hashCode * -1521134295 + Z.GetHashCode();
        return hashCode;
    }
}

class Tile : MonoBehaviour
{

    public readonly static float tileWidth = 1;
    public readonly static float tileHeight = 0.5f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    void OnLevelLoad()
    {

    }

    public static TilePosition GetTile(Vector3 world)
    {
        return new TilePosition(world.x, world.y, world.z);
    }

    public static TilePosition GetTile(int x, int y, int z)
    {
        return new TilePosition(x, y, z);
    }

    public static Entity GetEntityInTile(TilePosition tile)
    {
        foreach (Entity e in Entity.entities)
        {
            var t = GetTile(e.transform.position);
            if (t == tile) return e;
        }
        return null;
    }

    public static bool HasEntityInTile(TilePosition tile)
    {
        foreach (Entity e in Entity.entities)
        {
            var t = GetTile(e.transform.position);
            if (t == tile) return true;
        }
        return false;
    }
}

static class TileExtensions
{
    public static Entity GetEntity(this TilePosition tile)
    {
        return Tile.GetEntityInTile(tile);
    }
}