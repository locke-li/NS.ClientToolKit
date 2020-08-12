using System.IO;
using System.Text;
using CenturyGame.AppUpdaterLib.Runtime.Configs;
using UnityEditor;
using UnityEngine;

namespace CenturyGame.AppUpdaterLib.Editor
{
    public static class MenuCommands
    {

        private const string CreateAppUpdaterConfig = "CenturyGame/AppUpdaterLib/Create appupdater config";

        [MenuItem(CreateAppUpdaterConfig)]
        static void MakeDefaultConfig()
        {
            string targetFolderPath = Application.dataPath + "/CenturyGamePackageRes/AppUpdaterLib/Resources";
            if (!Directory.Exists(targetFolderPath))
            {
                Directory.CreateDirectory(targetFolderPath);

                AssetDatabase.Refresh();
            }

            var path = targetFolderPath + "/appupdater.txt";

            if (File.Exists(path))
            {
                Debug.LogWarning("The appupdater config that you want to make is exist in current project!");
                return;
            }
            AppUpdaterConfig config = new AppUpdaterConfig();
            string jsonContents = JsonUtility.ToJson(config, true);
            File.WriteAllText(path, jsonContents, new UTF8Encoding(false, true));

            Debug.Log("Write default appupdater default config completd!");

            AssetDatabase.Refresh();

        }
    }
}

