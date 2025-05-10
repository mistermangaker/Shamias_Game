
using UnityEngine;

public static class RectIntExtensions 
{

    public delegate void RectAction(Vector3Int coords);
    public delegate bool RectActionBool(Vector3Int coords);

    public static void Iterate(this RectInt rectInt, Vector3Int coords, RectAction action)
    {
        for (var x = rectInt.x; x < rectInt.xMax; x++)
        {
            for (var y = rectInt.y; y < rectInt.yMax; y++)
            {
                action(coords + new Vector3Int(x, y));
            }
        }
    }

    public static bool Iterate(this RectInt rectInt, Vector3Int coords, RectActionBool action)
    {
        for (var x = rectInt.x; x < rectInt.xMax; x++)
        {
            for (var y = rectInt.y; y < rectInt.yMax; y++)
            {
                if (action(coords + new Vector3Int(x, y)))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
