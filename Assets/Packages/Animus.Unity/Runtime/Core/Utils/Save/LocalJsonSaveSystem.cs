using System;
using System.IO;
using System.Linq;
using UnityEngine;
using JsonUtility = Packages.Animus.Unity.Runtime.Core.Utils.Json.JsonUtility;

namespace Packages.Animus.Unity.Runtime.Core.Utils.Save
{
    public static class LocalJsonSaveSystem
    {
        private static readonly string SaveDirectory = Path.Combine(Application.persistentDataPath, "saves");

        public static string Save<T>(T saveData, string fileName = null)
        {
            // Generate timestamp-based filename if none provided
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = $"save_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            }

            var savePath = ToSavePath(fileName);

            Directory.CreateDirectory(SaveDirectory);

            var json = JsonUtility.Serialize(saveData, prettyPrint: true);
            File.WriteAllText(savePath, json);
            return savePath;
        }

        public static string SaveJsonString(string json, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = $"save_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            }

            var savePath = ToSavePath(fileName);

            Directory.CreateDirectory(SaveDirectory);

            File.WriteAllText(savePath, json);

            return savePath;
        }

        public static T Load<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return default;
            }

            var loadPath = ToSavePath(fileName);

            if (!File.Exists(loadPath))
            {
                return default;
            }

            var json = File.ReadAllText(loadPath);
            return JsonUtility.Deserialize<T>(json);
        }

        public static bool Delete(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            var savePath = ToSavePath(fileName);

            if (!File.Exists(savePath))
            {
                return false;
            }

            File.Delete(savePath);
            return true;
        }

        public static string[] GetSaveFiles()
        {
            if (!Directory.Exists(SaveDirectory))
            {
                return Array.Empty<string>();
            }

            return Directory.GetFiles(SaveDirectory, "*.json").Select(Path.GetFileName).ToArray();
        }

        public static string GetLatestSaveFile()
        {
            return GetSaveFiles().OrderByDescending(File.GetLastWriteTime).ToArray().FirstOrDefault();
        }

        public static bool HasAnySave()
        {
            return GetSaveFiles().Length > 0;
        }

        public static bool SaveExists(string fileName)
        {
            return File.Exists(ToSavePath(fileName));
        }

        private static string ToSavePath(string fileName)
        {
            // Add .json extension if not present
            if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".json";
            }

            return Path.Combine(SaveDirectory, fileName);
        }
    }
}