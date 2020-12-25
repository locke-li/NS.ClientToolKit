using UnityEditor;
using UnityEngine;
using System.IO;

namespace CenturyGame.AppSetting.Editor
{
    class Menus
    {
        [MenuItem("CenturyGame/AppSetting/创建AppSetting")]
        static void CreateAppSetting()
        {
            var relativePath = $"Assets/CenturyGamePackageRes/AppSetting/Resources/AppSetting.asset";
            var path = $"{Application.dataPath}/../{relativePath}";
            path = Path.GetFullPath(path);
            path = path.Replace(@"\", "/");

            bool create = true;
            if (File.Exists(path))
            {
                if (!EditorUtility.DisplayDialog("创建App Setting 配置","是否覆盖现有的AppSetting？","OK","Cancle"))
                {
                    create = false;
                }
            }

            if (create)
            {
                var appSetting = ScriptableObject.CreateInstance<Runtime.AppSetting>();
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                AssetDatabase.CreateAsset(appSetting, relativePath);
            }
            else
            {
                Debug.LogWarning("You cancled to create the default appsetting! ");
            }
        }
    }
}


