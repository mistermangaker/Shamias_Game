using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(BoxCollider))]
public class BoundingBoxes : MonoBehaviour
{
    public List<string> tags;

    public bool HasTag(string tag)
    {
        foreach (var item in tags)
        {
            if(item == tag) return true;
        }
        return false;
    }
    [SerializeField]
    private BoxCollider boxcollider;

    private void Start()
    {
        boxcollider = GetComponent<BoxCollider>();
        boxcollider.isTrigger = true;
    }

    public Vector3 GetRandomXZPositionInBoundingBox()
    {
        Bounds bounds = boxcollider.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxcollider.bounds.center, boxcollider.bounds.size);
    }
}
