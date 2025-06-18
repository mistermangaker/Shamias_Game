using UnityEngine;

[CreateAssetMenu(fileName = "ThingTag", menuName = "Scriptable Objects/ThingTag")]
public class ThingTag : ScriptableObject
{
    [field: SerializeField] public string TagName { get; private set; }
}
