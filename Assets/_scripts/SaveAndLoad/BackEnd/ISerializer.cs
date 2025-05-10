using UnityEngine;

namespace GameSystems.SaveLoad
{
    public interface ISerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }
}

