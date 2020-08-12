/***************************************************************

 *  类名称：        EditorHelper

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/3 11:11:31

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using CenturyGame.Log4NetForUnity.Runtime;
using System.Text;
using System.Data.Common;

namespace CenturyGame.Log4NetForUnity.Editor
{

    public class Log4NetLinkXmlImportWindow : EditorWindow
    {
        public enum WindowType
        {
            Normal,
            LinkXmlFileNotFound,
            ReplaceLinkXml,            
        }
        private WindowType mType;

        public static void Show(WindowType type ,string title)
        {
            var window = GetWindow<Log4NetLinkXmlImportWindow>(false,title);
            window.minSize = new Vector2(1080,720);
            window.Show();


            window.mType = type;

        }


        private void OnGUI()
        {
            switch (this.mType)
            {
                case WindowType.LinkXmlFileNotFound:
                    this.DrawLinkXmlFileNotFound();
                    break;
                case WindowType.ReplaceLinkXml:
                    this.ReplaceLinkXml();
                    break;
                default:
                    break;

            }
        }

        private void DrawLinkXmlFileNotFound()
        {
            GUILayout.Label("当前引入的Log4Net包里不包含link.xml文件，请联系：Chico!");
        }


        private void ReplaceLinkXml()
        {
            GUILayout.Label("当前link.xml文件角旧，请覆盖此文件!");

            if (GUILayout.Button("覆盖当前的link.xml文件"))
            {
                try
                {
                    EditorHelper.CopyLinkXMl();
                    this.mType = WindowType.Normal;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }

    public sealed class EditorHelper
    {

        //[InitializeOnLoadMethod]
        //static void CheckLinkXmlValid()
        //{
        //    string sourcePath = Path.GetFullPath("Packages/com.centurygame.log4netforunity/link.xml");
        //    if (!File.Exists(sourcePath))
        //    {
        //        Log4NetLinkXmlImportWindow.Show(Log4NetLinkXmlImportWindow.WindowType.LinkXmlFileNotFound, "Link.xml not found !");

        //        return;
        //    }

        //    var targetPath = $"{Application.dataPath}/CenturyGamePackageRes/Log4NetForUnity/link.xml";
        //    if (!File.Exists(targetPath))
        //    {
        //        string xml = File.ReadAllText(sourcePath, new UTF8Encoding(false, true));
        //        File.WriteAllText(targetPath, xml, new UTF8Encoding(false, true));
        //        Debug.Log($"Copy link.xml file success , xml path : {targetPath} .");

        //        return;
        //    }

        //    string sourceMd5 = GetMD5(sourcePath);

        //    string targetMd5 = GetMD5(targetPath);

        //    if (sourceMd5 != targetMd5)
        //    {
        //        Debug.LogError($"Current md5 : {sourceMd5} , newest md5 : {targetMd5} .");
        //        Log4NetLinkXmlImportWindow.Show(Log4NetLinkXmlImportWindow.WindowType.ReplaceLinkXml, "Replace old link.xml");
        //    }

            
        //}


        private static string GetMD5(string filePath)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = bytes;
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private const string CopyLog4NetLinkXml = "CenturyGame/log4net/Copy link.xml to log4netforunity contents folder";
        [MenuItem(CopyLog4NetLinkXml, priority = 1)]
        public static void CopyLinkXMl()
        {
            string path = Path.GetFullPath("Packages/com.centurygame.log4netforunity/link.xml");

            if (!File.Exists(path))
            {
                Debug.LogError("The link.xml that we want to copy is not exist in the current package , try to reinstall package!");
            }

            

            string targetFolderPath = $"{Application.dataPath}/CenturyGamePackageRes/Log4NetForUnity"; ;
            if (!Directory.Exists(targetFolderPath))
            {
                Directory.CreateDirectory(targetFolderPath);

                AssetDatabase.Refresh();
            }

            var targetPath = $"{Application.dataPath}/CenturyGamePackageRes/Log4NetForUnity/link.xml";
            string xml = File.ReadAllText(path, new UTF8Encoding(false, true));
            File.WriteAllText(targetPath, xml, new UTF8Encoding(false, true));
            Debug.Log($"Copy link.xml file success , xml path : {targetPath} .");

            AssetDatabase.Refresh();
        }

        private const string CreateDefaultLog4NetConfig = "CenturyGame/log4net/Make Default Config";

        [MenuItem(CreateDefaultLog4NetConfig, priority = 2)]
        static void MakeDefaultConfig()
        {
            string targetFolderPath = Application.dataPath + "/CenturyGamePackageRes/Log4NetForUnity/Resources";
            if (!Directory.Exists(targetFolderPath))
            {
                Directory.CreateDirectory(targetFolderPath);

                AssetDatabase.Refresh();
            }

            var path = targetFolderPath + "/log4net.xml";

            if (File.Exists(path))
            {
                Debug.LogWarning("The log4net xml config that you want to make is exist in current project!");
                return;
            }

            File.WriteAllText(path, Const.DefaultConfig, new UTF8Encoding(false, true));

            Debug.Log("Write default log4Net default config completd!");

            AssetDatabase.Refresh();
        }


    }
}
