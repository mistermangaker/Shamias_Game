using UnityEngine;

public abstract class ScriptableObjectWithId : ScriptableObject
{
    [field: SerializeField] public virtual string Id { get; private set; }

}
