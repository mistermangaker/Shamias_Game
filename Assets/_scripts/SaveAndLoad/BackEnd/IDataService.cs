using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.SaveLoad
{
    public interface IDataService
    {
        void Save(GameData data, bool overwrite = true);
        void Save(GameData data, bool overwrite = true, string newname ="");
        GameData Load(string name);
        void Delete(string name);
        IEnumerable<string> ListSaves();

        IEnumerable<string> ListSimiliarSaves();
    }
}

