using Callendar;
using GameSystems.SaveLoad;
using System;
using UnityEngine;
[Serializable]
public class PlayerSaveData : ISaveable
{
    [field:SerializeField] public SerializableGuid Id { get; set; }
    [SerializeField] public Vector3 Position;
    [SerializeField] public Vector3 FactingDirections;
}

