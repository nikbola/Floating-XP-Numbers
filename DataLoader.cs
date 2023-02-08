using System.IO;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace FloatingXPNumbers
{
    static class DataLoader
    {
        public static void Load(UnityModManager.ModEntry modEntry)
        {
            string path = Path.Combine(Assembly.GetExecutingAssembly().Location, @"..\Settings.json");
            string jsonString = File.ReadAllText(path);
            Storage.LoadSettings settings = JsonUtility.FromJson<Storage.LoadSettings>(jsonString);
            if (settings != null && settings.floatDuration > 0)
            {
                Storage.floatDuration = settings.floatDuration;
                modEntry.Logger.Log("Successfully loaded settings from " + path + ".");
                modEntry.Logger.Log("Float duration set to " + Storage.floatDuration);
            }
            else
            {
                Storage.floatDuration = 4f;
                modEntry.Logger.Error("Could not parse settings file at " + path + ".");
                modEntry.Logger.Error("Using default values.");
            }
        }
    }
}
