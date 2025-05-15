using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine;

[Serializable]
public class BuildingSerializableGUID : MonoBehaviour, ISaveable
{


    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

    public void SetId(SerializableGuid id)
    {
        Id = id;
    }

    [ContextMenu("Set New ID")]
    public void SetNewId()
    {
        Id = SerializableGuid.NewGuid();
    }

   

}
