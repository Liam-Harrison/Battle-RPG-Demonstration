using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class Extensions
{
    public static Vector3 RemoveAxisAndNormalize(this Vector3 i, bool X = false, bool Y = false, bool Z = false)
    {
        if (X) i.x = 0;
        if (Y) i.y = 0;
        if (Z) i.z = 0;
        return i.normalized;
    }

    public static Vector3 RemoveAxis(this Vector3 i, bool X = false, bool Y = false, bool Z = false)
    {
        if (X) i.x = 0;
        if (Y) i.y = 0;
        if (Z) i.z = 0;
        return i;
    }

    public static TilePosition GetTile(this Entity e)
    {
        return Tile.GetTile(e.transform.position);
    }
}
