using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace GameSystems.SaveLoad
{
    public class FileDataService : IDataService
    {
        ISerializer serializer;
        string dataPath;
        string fileExtention;
        public FileDataService(ISerializer serializer)
        {
            this.dataPath = Path.Combine(Application.dataPath,"GameSaves");
            this.fileExtention = "json";
            this.serializer = serializer;
        }
        string GetPathToFile(string filename)
        {
            if (!Directory.Exists(dataPath)){
                Directory.CreateDirectory(dataPath);
            }
            return Path.Combine(dataPath, string.Concat(filename, ".", fileExtention));
        }
        public void Save(GameData data, bool overwrite)
        {
            Save(data, overwrite, data.Name);
        }
        public void Save(GameData data, bool overwrite, string newname)
        {
            string fileLoction = GetPathToFile(newname);
            //GUIUtility.systemCopyBuffer = fileLoction;
            if (!overwrite && File.Exists(fileLoction))
            {
                throw new IOException($"the file at '{data.Name}.{fileExtention}' already exists and cannot be overwritten");
            }
            File.WriteAllText(fileLoction, serializer.Serialize(data));
        }

        public GameData Load(string name)
        {
            string fileLoction = GetPathToFile(name);
            if (!File.Exists(fileLoction))
            {
                throw new System.Exception($"no persisted data with name'{name}'");
            }
            return serializer.Deserialize<GameData>(File.ReadAllText(fileLoction));
        }

        public void Delete(string name)
        {
            string fileLoction = GetPathToFile(name);
            if (File.Exists(fileLoction)){
                File.Delete(fileLoction);
            }
        }

        public IEnumerable<string>ListSaves()
        {
            foreach(string path in Directory.EnumerateFiles(dataPath))
            {
                if(Path.GetExtension(path) == "."+fileExtention)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }

        public IEnumerable<string> ListSimiliarSaves()
        {
            foreach (string path in Directory.EnumerateFiles(dataPath))
            {
                if (Path.GetExtension(path) == "." + fileExtention)
                {
                    yield return path;
                }
            }
        }
    }

}
