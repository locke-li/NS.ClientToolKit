//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.IO;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Security.Cryptography;
//using System;
//using System.Linq;
//using System.Net.Http.Headers;
//using CenturyGame.AssetBundleManager.Runtime;
//using Unity.AssetGraph.DataModel.Version2;
//using UnityEditor.SceneManagement;
//using UnityEngine.SceneManagement;
//using Object = UnityEngine.Object;
//using CenturyGame.Editor.UploadUtilitis.WinSCP;

//#if DEBUG_FILE_CRYPTION
//using FileEncryption = CenturyGame.AssetBundleManager.Runtime.Core.IO.File;
//#else
//using FileEncryption = System.IO.File;
//#endif



///// <summary>
///// AB系统构建类
///// </summary>
//public partial class ABMenu
//{
//    public static readonly string LastBuildVersionPath = "Assets/Standard Assets/AssetBundleManager/",
//    LastBuildVersionName = "LastBuildVersion.x",
//    AssetsTitle = "Assets:",
//    AbExportFolder = "AB",
//    StreamingAssetsFolder = "Assets/StreamingAssets",
//    DependenciesTitle = "Dependencies:",
//    DependenciesTitleError = "Dependencies: []",
//    ManifestFineTitle = "Name: ",
//    PackagePath = "Package",
//    PatternManifest = ".manifest",
//    AbBuildConfigName = "Assets/Standard Assets/AssetGraph/Test_Build.asset",
//    VersionStar = "R",
//    VersionFormat = "yyyyMMddHHmmss";
    
//    private static string currentVer = null;
//    private static bool uploadToServer = false;
//    private static string lastBuildVer = null;
    
////    [MenuItem("M5/资源管理/版本 Ab清单")]
////    public static void BuildAb()
////    {
////        long tick = DateTime.Now.Ticks;
////        List<FileInfo> sceneList = CopyScene();
////        DeleteScene(sceneList);
////        BackBuildSceneSetting();
////        AssetDatabase.Refresh();
////        tick = DateTime.Now.Ticks - tick;
////        Debug.Log(tick / 10000);
////        BuildAb(true);
// //   }
    
//    [MenuItem("FP/资源管理/打包Ab窗口")]
//    public static void BuildAbAll()
//    {
////        DirectoryInfo folder = new DirectoryInfo(string.Concat(System.Environment.CurrentDirectory, Path.DirectorySeparatorChar, "Assets"));
////        if (folder.Exists)
////        {
////            FileInfo[] files = folder.GetFiles("*.DDS", SearchOption.AllDirectories);
////            Debug.Log(files.Length);
////        }

//        ABMenuUI.ShowWindow();
//    }
    
//    //[MenuItem("Assets/资源管理/启动AB场景 &g")]
//    //static void loadABScene()
//    //{
//    //    if (!UnityEditor.EditorApplication.isPlaying)
//    //    {
//    //        EditorSceneManager.OpenScene("Assets/Scenes/ab.unity");
//    //        UnityEditor.EditorApplication.isPlaying = true;
//    //    }
//    //}
    
//    [MenuItem("FP/资源管理/获取SHA1")]
//    public static void GetSha1()
//    {
//        Object o = Selection.activeObject;
//        string path = AssetDatabase.GetAssetPath(o);
//        string targetFile = string.Concat(System.Environment.CurrentDirectory
//            , Path.DirectorySeparatorChar
//            , path);
//        ;
//        Debug.Log(ABBuildCache.GetHash(targetFile));
//    }
//    /// <summary>
//    /// 打包调用
//    /// </summary>
//    /// <param name="upload"></param>
//    public static void BuildAb(bool upload = true)
//    {
//        uploadToServer = upload;
//        deleteBulidVersionInfo();
//        makePatch(true);
//    }

//    public static void CreatePatch()
//    {
//        CreatePatch(true);
//    }
//    /// <summary>
//    /// 打Patch调用
//    /// </summary>
//    /// <param name="upload"></param>
//    public static void CreatePatch(bool upload = true)
//    {
//        uploadToServer = upload;
//        makePatch(true);
//        AbHelp.BackBuildSceneSetting();
//    }
    
//    /// <summary>
//    /// 保存版本号
//    /// </summary>
//    public static void SaveBulidVersionInfo()
//    {
//        Debug.Log("ABMenu.SaveBulidVersionInfo()...");
////        string scriptPath = string.Concat(System.Environment.CurrentDirectory
////         , Path.DirectorySeparatorChar
////         , "Library"
////         , Path.DirectorySeparatorChar
////         , "ScriptAssemblies"
////         , Path.DirectorySeparatorChar
////         , "Assembly-CSharp.dll");

//        string confInfoPath = string.Concat(System.Environment.CurrentDirectory
//            , Path.DirectorySeparatorChar
//            , StreamingAssetsFolder
//            , Path.DirectorySeparatorChar
//            , AbHelp.AbConfigInfoName);

//        //if (File.Exists(confInfoPath) && File.Exists(scriptPath))
//        if (File.Exists(confInfoPath))
//        {
//            LastBuildVersion lastVersion = new LastBuildVersion();
//            //lastVersion.CodeVer = GetHash(scriptPath, HashType.SHA1);
//            lastVersion.CodeVer = "9527";
//            lastVersion.AbVer = currentVer;
//            lastVersion.Info = new ABConfigInfoClient(JsonUtility.FromJson<ABConfigInfo>(File.ReadAllText(confInfoPath)));
//            string targetFile = string.Concat(System.Environment.CurrentDirectory
//                , Path.DirectorySeparatorChar
//                , LastBuildVersionPath
//                , LastBuildVersionName);
//            var dat = JsonUtility.ToJson(lastVersion, true);
//            FileEncryption.WriteAllBytes(targetFile, System.Text.Encoding.UTF8.GetBytes(dat));
//            Debug.Log("ABMenu.SaveBulidVersionInfo write file:" + targetFile);
//        }
//        else
//        {
//            Debug.LogError("ABMenu.CheckBulidVersionInfo no configinfo!");
//        }
//    }
    
//    /// <summary>
//    /// 删除版本信息
//    /// </summary>
//    private static void deleteBulidVersionInfo()
//    {
//        Debug.Log("ABMenu.deleteBulidVersionInfo...");
//        FileInfo targetFile = new FileInfo(string.Concat(System.Environment.CurrentDirectory
//                , Path.DirectorySeparatorChar
//                , LastBuildVersionPath
//                , LastBuildVersionName));
//        if(targetFile.Exists)
//        {
//            LastBuildVersion lastVersion = JsonUtility.FromJson<LastBuildVersion>(File.ReadAllText(targetFile.FullName, Encoding.UTF8));
//            if (!string.IsNullOrEmpty(lastVersion.AbVer))
//            {
//                lastBuildVer = lastVersion.AbVer;
//                string movePath = string.Concat(System.Environment.CurrentDirectory
//                    , Path.DirectorySeparatorChar
//                    , PackagePath
//                    , Path.DirectorySeparatorChar
//                    , lastBuildVer
//                    , Path.DirectorySeparatorChar);
//                if(Directory.Exists(movePath))
//                {
//                    targetFile.CopyTo(string.Concat(movePath, LastBuildVersionName), true);
//                    Debug.Log("ABMenu.deleteBulidVersionInfo ok");
//                }
//            }
//            targetFile.Attributes = FileAttributes.Normal;
//            targetFile.Delete();
//            FileInfo targetFileMeta = new FileInfo(string.Concat(targetFile.FullName, AbHelp.FileExt[0]));
//            if (targetFileMeta.Exists)
//            {
//                targetFileMeta.Attributes = FileAttributes.Normal;
//                targetFileMeta.Delete();
//            }
//        }
//        else
//        {
//            Debug.Log("ABMenu.deleteBulidVersionInfo first build!");
//        }
//    }
    
//    /// <summary>
//    /// 打包失败，回退版本信息
//    /// </summary>
//    public static void BackBulidVersionInfo()
//    {
//        Debug.Log("ABMenu.BackBulidVersionInfo...");
//        if (lastBuildVer != null)
//        {
//            FileInfo sourceFile = new FileInfo(string.Concat(System.Environment.CurrentDirectory
//                , Path.DirectorySeparatorChar
//                , PackagePath
//                , Path.DirectorySeparatorChar
//                , lastBuildVer
//                , Path.DirectorySeparatorChar
//                , LastBuildVersionName));
//            FileInfo targetPath = new FileInfo(string.Concat(System.Environment.CurrentDirectory
//                , Path.DirectorySeparatorChar
//                , LastBuildVersionPath
//                , LastBuildVersionName));
//            if (sourceFile.Exists && !targetPath.Exists)
//            {
//                sourceFile.MoveTo(targetPath.FullName);
//                Debug.Log("ABMenu.BackBulidVersionInfo ok");
//            }
//        }
//        else
//        {
//            Debug.Log("ABMenu.BackBulidVersionInfo no last build ver!");
//        }
//        lastBuildVer = null;
//    }
//    public static bool CheckBulidVersionInfo()
//    {
//        FileInfo targetFile = new FileInfo(string.Concat(System.Environment.CurrentDirectory
//                , Path.DirectorySeparatorChar
//                , LastBuildVersionPath
//                ,LastBuildVersionName));
//        if (!targetFile.Exists)
//        {
//            Debug.LogError("ABMenu.CheckBulidVersionInfo targetFile is not exists!");
//            return false;
//        }

//        string scriptPath = string.Concat(System.Environment.CurrentDirectory
//         , Path.DirectorySeparatorChar
//         , "Library"
//         , Path.DirectorySeparatorChar
//         , "ScriptAssemblies"
//         , Path.DirectorySeparatorChar
//         , "Assembly-CSharp.dll");
//        if(!File.Exists(scriptPath))
//        {
//            Debug.LogError("ABMenu.CheckBulidVersionInfo c# dll is not exists!");
//            return false;
//        }

//        LastBuildVersion lastVersion = JsonUtility.FromJson<LastBuildVersion>(File.ReadAllText(targetFile.FullName, Encoding.UTF8));
//        string currentVersion = ABBuildCache.GetHash(scriptPath);
//        if (string.Equals(currentVersion, lastVersion.CodeVer))
//        {
//            return true;
//        }
//        else
//        {
//            Debug.LogError("ABMenu.CheckBulidVersionInfo c# dll is changed!");
//            return false;
//        }
//    }
    
//    /// <summary>
//    /// 打Path包
//    /// </summary>
//    /// <param name="makeLua"></param>
//    static void makePatch(bool makeLua)
//    {
//        AbHelp.IsUseAB = false;
//        List<FileInfo> luaList = null;
//        List<FileInfo> dataList = null;
//        List<FileInfo> sceneList = null;
//        if(makeLua)
//        {
//            //EditorUtility.DisplayCancelableProgressBar("AB系统工作中...", "整理Lua和配置表...", 0.0f);
//            //luaList = CopyLua();
//            //dataList = CopyData();
//            //sceneList = CopyScene();
//            //AssetDatabase.Refresh();
//        }

//        //Unity.AssetGraph.AssetGraphEditorWindow.BuildFromMenu();
//        EditorUtility.DisplayCancelableProgressBar("AB系统工作中...", "打包AB文件。", 0.1f);
//        Unity.AssetGraph.AssetGraphUtility.ExecuteGraph(AbBuildConfigName);
//        EditorUtility.DisplayCancelableProgressBar("AB系统工作中...", "创建文件清单。", 0.5f);
//        createList();
//        if (makeLua)
//        {
//            //EditorUtility.DisplayCancelableProgressBar("AB系统工作中...", "删除临时Lua和配置表...", 0.55f);
//            //DeleteFile(luaList);
//            //DeleteFile(dataList);
//            //DeleteScene(sceneList);
//            //AssetDatabase.Refresh();
//        }
//        EditorUtility.DisplayCancelableProgressBar("AB系统工作中...", "生成与上个版本的差异清单。", 0.6f);
//        mergerFile();
//        EditorUtility.ClearProgressBar();
//    }

//    //这个值一般不需要更改
//    static DateTime startPointOfBuild = new DateTime(2020, 4, 23, 18, 0, 0, 0);

//    /// <summary>
//    /// 创建文件清单
//    /// </summary>
//    static void createList()
//    {
//        currentVer = string.Concat(VersionStar, DateTime.Now.ToString(VersionFormat));
//        //AssetBundle Graph 输出目录
//        DirectoryInfo disk = new DirectoryInfo(string.Concat(System.Environment.CurrentDirectory, Path.DirectorySeparatorChar, AbExportFolder));
//        FileInfo manifest = new FileInfo(string.Concat(disk.FullName, Path.DirectorySeparatorChar, AbExportFolder, PatternManifest));
//        if (disk.Exists && manifest.Exists)
//        {
//            FileInfo manifestFile = new FileInfo(string.Concat(System.Environment.CurrentDirectory, Path.DirectorySeparatorChar, ABMenu.AbExportFolder, Path.DirectorySeparatorChar, ABMenu.AbExportFolder));
//            AssetBundleManifest mainAbManifest = null;
//            AssetBundle mainAb = null;
//            if (manifest.Exists)
//            {
//                byte[] bytes = File.ReadAllBytes(manifestFile.FullName);
//                mainAb = AssetBundle.LoadFromMemory(bytes); 
//                mainAbManifest = mainAb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
//                mainAb.Unload(false);
//            }

//            int pathLength = disk.FullName.Length;
//            string targetPath = string.Concat(System.Environment.CurrentDirectory, Path.DirectorySeparatorChar, StreamingAssetsFolder);
//            List<ABTableItem> list = new List<ABTableItem>();
//            List<ABTableItemInfo> infoList = new List<ABTableItemInfo>();

//            Dictionary<string, bool> clearList = clearFilesGet(disk, manifest);
//            List<FileInfo> files = getFiles(manifest, clearList);
//            clearFilesRemove(clearList, new[] { "", PatternManifest});
//            clearStreamingAssets(targetPath);
//            for (int i = 0; i < files.Count; i++)
//            {
//                FileInfo tmpFile = files[i];
//                readManifest(tmpFile, list, infoList, pathLength, mainAbManifest);
//                copyFile(tmpFile, disk.FullName, targetPath);
//            }
            
//            ABConfig config = new ABConfig();
//            ABConfigInfo configInfo = new ABConfigInfo();
//            config.Tables = list.ToArray();
//            configInfo.List = infoList.ToArray();
//            configInfo.Ver = currentVer;
//            string confPath = string.Concat(System.Environment.CurrentDirectory
//                , Path.DirectorySeparatorChar
//                , StreamingAssetsFolder
//                , Path.DirectorySeparatorChar
//                , AbHelp.AbConfigName);
//            string confInfoPath = string.Concat(System.Environment.CurrentDirectory
//                , Path.DirectorySeparatorChar
//                , StreamingAssetsFolder
//                , Path.DirectorySeparatorChar
//                , AbHelp.AbConfigInfoName);

//            try
//            {
//                var dat = JsonUtility.ToJson(config);
//                FileEncryption.WriteAllBytes(confPath, System.Text.Encoding.UTF8.GetBytes(dat));

//                dat = JsonUtility.ToJson(configInfo);
//                FileEncryption.WriteAllBytes(confInfoPath, System.Text.Encoding.UTF8.GetBytes(dat));
//                Debug.LogWarning("保存设置成功！");
//            }
//            catch (System.Exception ex)
//            {
//                Debug.LogError("保存设置失败，请确认文件读写权限。" + ex);
//            }
//            //copyDirectory(string.Concat(disk.FullName, Path.DirectorySeparatorChar)
//                //, string.Concat(System.Environment.CurrentDirectory, Path.DirectorySeparatorChar, StreamingAssetsFolder));
//        }
//    }
    
///// <summary>
///// 根据manifest获取文件清单
///// </summary>
///// <param name="manifest"></param>
///// <param name="clearList"></param>
///// <returns></returns>
//    static List<FileInfo> getFiles(FileInfo manifest, Dictionary<string, bool> clearList)
//    {
//        List<FileInfo> files = new List<FileInfo>();
//        string abPath = Path.GetDirectoryName(manifest.FullName);
//        using (TextReader textReader = new StreamReader(manifest.OpenRead(), Encoding.UTF8))
//        {
//            while (true)
//            {
//                string line = textReader.ReadLine();
//                if (line == null)
//                    break;
//                int index = line.IndexOf(ManifestFineTitle, StringComparison.Ordinal);
//                if (index > -1)
//                {
//                    line = line.Substring(index + ManifestFineTitle.Length);
//                    FileInfo tmpFile = new FileInfo(string.Concat(abPath, Path.DirectorySeparatorChar, line, PatternManifest));
//                    if (tmpFile.Exists)
//                    {
//                        files.Add(tmpFile);
//                        clearFilesCheck(clearList, tmpFile);
//                    }
//                }
//            }
//        }
//        return files;
//    }

//    static Dictionary<string, bool> clearFilesGet(DirectoryInfo folder, FileInfo manifest)
//    {
//        FileInfo[] files = folder.GetFiles("*.*", SearchOption.AllDirectories);
//        Dictionary<string, bool> clearList = new Dictionary<string, bool>();
//        for (int i = 0; i < files.Length; i++)
//        {
//            FileInfo info = files[i];
//            clearList[info.FullName] = true;
//        }

//        clearFilesCheck(clearList, manifest);
//        return clearList;
//    }


//    static void clearFilesCheck(Dictionary<string, bool> clearList, FileInfo file)
//    {
//        if (clearList.ContainsKey(file.FullName))
//        {
//            clearList.Remove(file.FullName);
//            string Ext = Path.GetExtension(file.FullName);
//            string sourceFile = file.FullName.Substring(0, file.FullName.Length - Ext.Length);
//            if (clearList.ContainsKey(sourceFile))
//            {
//                clearList.Remove(sourceFile);
//            }
//        }
//    }
//    static void clearFilesRemove(Dictionary<string, bool> clearList, string[] searchList)
//    {
//        foreach (var VARIABLE in clearList)
//        {
//            string flie = VARIABLE.Key;
//            bool delete = false;
//            for (int i = 0; i < searchList.Length; i++)
//            {
//                if (Path.GetExtension(flie) == searchList[i])
//                {
//                    delete = true;
//                    break;
//                }
//            }
//            if (delete)
//            {
//                File.Delete(flie);
//            }
//        }
//    }
//    static void clearStreamingAssets(string folder)
//    {
//        DirectoryInfo disk = new DirectoryInfo(folder);
//        if (disk.Exists)
//        {
//            FileInfo[] files = disk.GetFiles("*.*", SearchOption.AllDirectories);
//            for (int i = 0; i < files.Length; i++)
//            {
//                FileInfo info = files[i];
//                if (info.Extension == AbHelp.AbFileExt)
//                {
//                    info.Delete();
//                }
//                else if (info.Extension == ".meta")
//                {
//                    string sourceFile = info.FullName.Substring(0, info.FullName.Length - info.Extension.Length);
//                    if (Path.GetExtension(sourceFile) == AbHelp.AbFileExt)
//                    {
//                        info.Delete();
//                    }
//                }
//            }
//        }

//    }
//    static bool isHexadecimal(string str)
//    {
//        const string PATTERN = @"^[A-Fa-f0-9]+$";
//        return System.Text.RegularExpressions.Regex.IsMatch(str, PATTERN);
//    }
    
//    /// <summary>
//    /// 读取单个AB文件配置
//    /// </summary>
//    /// <param name="tmpFile"></param>
//    /// <param name="list"></param>
//    /// <param name="infoList"></param>
//    /// <param name="pathLength"></param>
//    /// <param name="mainAbManifest"></param>
//    static void readManifest(FileInfo tmpFile, List<ABTableItem> list, List<ABTableItemInfo> infoList, int pathLength, AssetBundleManifest mainAbManifest)
//    {
//        ABTableItem item = new ABTableItem();
//        ABTableItemInfo itemInfo = new ABTableItemInfo();
//        int index = tmpFile.FullName.LastIndexOf(".", StringComparison.Ordinal);
//        FileInfo abFileInfo = new FileInfo(tmpFile.FullName.Substring(0, index));
//        index = abFileInfo.Name.LastIndexOf(".", StringComparison.Ordinal);
//        if (abFileInfo.Exists)
//        {
//            itemInfo.S = (int)abFileInfo.Length;
//            itemInfo.H = ABBuildCache.GetHash(abFileInfo.FullName, ABBuildCache.HashType.SHA1);
//        }
//        //string abName = abFileInfo.Name.Substring(0, index == -1 ? abFileInfo.Name.Length : index);
//        abFileInfo = null;
//        int tLen = tmpFile.FullName.Length - pathLength - PatternManifest.Length;
//        string rName = tmpFile.FullName.Substring(pathLength + 1, tLen);
//        index = rName.LastIndexOf(".", StringComparison.Ordinal);
//        if(index > -1)
//        {
//            rName = rName.Substring(0, index);
//        }  
//        item.Name = rName.Replace(Path.DirectorySeparatorChar, '/');
        
//        itemInfo.N = item.Name;

//        int pathStar = item.Name.LastIndexOf("/", StringComparison.Ordinal);
//        string miniName = pathStar == -1 ? item.Name : item.Name.Substring(pathStar);

//        bool isHash = isHexadecimal(miniName) && miniName.Length == 32;

//        List<string> assets = new List<string>();
//        //List<string> dependencies = new List<string>();
//        using (TextReader textReader = new StreamReader(tmpFile.OpenRead(), Encoding.UTF8))
//        {
//            byte state = 0;
//            while (true)
//            {
//                string line = textReader.ReadLine();
//                if (line == null)
//                    break;
//                else
//                {
//                    if (string.CompareOrdinal(line, AssetsTitle) == 0)
//                    {
//                        state = 1;
//                        continue;
//                    }
//                    else if (string.CompareOrdinal(line, DependenciesTitle) == 0)
//                    {
//                        state = 2;
//                        continue;
//                    }
//                    else if (string.CompareOrdinal(line, DependenciesTitleError) == 0)
//                    {
//                        state = 0;
//                        break;
//                    }
//                    switch (state)
//                    {
//                        case 1:
//                            {
//                                if (!isHash)
//                                {
//                                    string str = line.Substring(2);

//                                    if (item.AbsPath == null)
//                                    {
//                                        //string indexStr = string.Concat("/", abName);
//                                        pathStar = str.IndexOf(item.Name, StringComparison.OrdinalIgnoreCase) + item.Name.Length;
//                                        item.AbsPath = str.Substring(0, pathStar);
//                                    }

//                                    if (pathStar < str.Length)
//                                    {
//                                        int extIndex = line.LastIndexOf(".", StringComparison.Ordinal);
//                                        if(extIndex > -1)
//                                        {
//                                            string extFile = line.Substring(extIndex);
//                                            //去掉.cginc文件
//                                            if (!AbHelp.CheckFileExt(extFile))
//                                            {
//                                                assets.Add(str.Substring(pathStar));
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        Debug.LogError("readManifest:" + tmpFile.FullName + "," + line);
//                                    }
//                                }
//                                break;
//                            }
//                        case 2:
//                            {
////                                string dependenc = line.Substring(pathLength + 3);
////                                index = dependenc.LastIndexOf(".", StringComparison.Ordinal);
////                                if (index > -1)
////                                    dependenc = dependenc.Substring(0, index);
////                                dependencies.Add(dependenc);
//                                break;
//                            }
//                    }
//                }
//            }
//            textReader.Close();
//        }
        
//        if (assets.Count > 0)
//        {
//            item.Assets = assets.ToArray();
//        }
//        //if (dependencies.Count > 0)
//        //item.Dependencies = dependencies.ToArray();
//        item.Dependencies = mainAbManifest.GetAllDependencies(item.Name);
//        list.Add(item);
//        infoList.Add(itemInfo);
//    }
//    private static void copyFile(FileInfo tmpFile, string sourcrPath, string targetPtah)
//    {
//        string extPath = tmpFile.FullName.Substring(sourcrPath.Length + 1);
//        extPath = extPath.Substring(0, extPath.Length - PatternManifest.Length);
//        string[] extPaths = extPath.Split(new char[] { Path.DirectorySeparatorChar });

//        int count = extPaths.Length - 1;
//        if (count >= 0)
//        {
//            for(int i = 0; i < count; i++)
//            {
//                targetPtah = string.Concat(targetPtah, Path.DirectorySeparatorChar, extPaths[i]);
//                if (!Directory.Exists(targetPtah))
//                {
//                    Directory.CreateDirectory(targetPtah);
//                }
//            }
//            targetPtah = string.Concat(targetPtah, Path.DirectorySeparatorChar, extPaths[count]);
//        }
//        targetPtah = string.Concat(targetPtah, AbHelp.AbFileExt);
//        string sourcePath = string.Concat(sourcrPath, Path.DirectorySeparatorChar, extPath);
//        File.Copy(sourcePath, targetPtah, true); 
//    }
//    private static void mergerFile()
//    {
//        FileInfo targetFile = new FileInfo(string.Concat(System.Environment.CurrentDirectory
//                , Path.DirectorySeparatorChar
//                , LastBuildVersionPath
//                , LastBuildVersionName));
//        List<ABTableItemInfo> packageFileList = new List<ABTableItemInfo>();
//        if (targetFile.Exists)
//        {
//            LastBuildVersion target = JsonUtility.FromJson<LastBuildVersion>(File.ReadAllText(targetFile.FullName, Encoding.UTF8));
//            string confInfoStrPath = string.Concat(System.Environment.CurrentDirectory
//                , Path.DirectorySeparatorChar
//                , StreamingAssetsFolder
//                , Path.DirectorySeparatorChar
//                , AbHelp.AbConfigInfoName);
//            Dictionary<string, ABTableItemInfoClient> targetList = new Dictionary<string, ABTableItemInfoClient>();
            
//            if (File.Exists(confInfoStrPath))
//            {
//                ABConfigInfo configInfo = JsonUtility.FromJson<ABConfigInfo>(File.ReadAllText(confInfoStrPath, Encoding.UTF8));
//                for(int i = 0; i < target.Info.List.Length; i++)
//                {
//                    ABTableItemInfoClient info = target.Info.List[i];
//                    targetList[info.N] = info;
//                }
//                for (int i = 0; i < configInfo.List.Length; i++)
//                {
//                    ABTableItemInfo info = configInfo.List[i];
//                    ABTableItemInfoClient client = null;
//                    if(targetList.ContainsKey(info.N))
//                    {
//                        client = targetList[info.N];
//                        if (client.H == info.H)
//                        {
//                            if (client.R)
//                            {
//                                packageFileList.Add(info);
//                            }
//                        }
//                        else
//                        {
//                            packageFileList.Add(info);
//                            client.R = true;
//                        }
//                    }
//                    else
//                    {
//                        client = new ABTableItemInfoClient(info);
//                        client.R = true;
//                        targetList[info.N] = client;
//                        packageFileList.Add(info);
//                    }
//                }
//            }

//            if (packageFileList.Count > 0)
//            {
//                target.Info.List = targetList.Values.ToArray();
//                var dat = JsonUtility.ToJson(target, true);
//                FileEncryption.WriteAllBytes(targetFile.FullName, System.Text.Encoding.UTF8.GetBytes(dat));
//                Debug.Log("hotfix save LastBuildVersion:" + targetFile.FullName + ", file cout:" + packageFileList.Count);
//            }
//        }
//        else
//        {
//            Debug.LogWarning("No last build version.");
//        }
//        makePackage(packageFileList, targetFile);
//    }
    
//    /// <summary>
//    /// 生成差异Patch
//    /// </summary>
//    /// <param name="list"></param>
//    /// <param name="lastBulid"></param>
//    private static void makePackage(List<ABTableItemInfo> list, FileInfo lastBulid)
//    {
//        HotFixConfig hotfixConfig = new HotFixConfig();
//        List<SshTask> sshTaskList = new List<SshTask>();
//        StringBuilder sb = new StringBuilder();
//        int totalSize = 0;
//        sb.AppendLine("Make package file count:" + list.Count);

//        string packagePath = string.Concat(System.Environment.CurrentDirectory
//             , Path.DirectorySeparatorChar
//             , PackagePath);

//        string packageNamePath = string.Concat(packagePath
//             , Path.DirectorySeparatorChar,
//             currentVer);
//        getSshTask(sshTaskList, packagePath, packageNamePath);

//        DirectoryInfo directoryInfo = new DirectoryInfo(packageNamePath);
//        if(directoryInfo.Exists)
//            directoryInfo.Delete(true);
//        directoryInfo.Create();

//        string confPath = string.Concat(System.Environment.CurrentDirectory
//            , Path.DirectorySeparatorChar
//            , StreamingAssetsFolder
//            , Path.DirectorySeparatorChar
//            , AbHelp.AbConfigName);
//        string confInfoPath = string.Concat(System.Environment.CurrentDirectory
//            , Path.DirectorySeparatorChar
//            , StreamingAssetsFolder
//            , Path.DirectorySeparatorChar
//            , AbHelp.AbConfigInfoName);

//        if (File.Exists(confPath) && File.Exists(confInfoPath))
//        {
//            string streamingPath = string.Concat(System.Environment.CurrentDirectory
//                , Path.DirectorySeparatorChar
//                , StreamingAssetsFolder
//                , Path.DirectorySeparatorChar);

//            for (int i = 0; i < list.Count; i++)
//            {
//                ABTableItemInfo info = list[i];
//                sb.AppendLine(info.N + ":" + AbHelp.GetSizeStr(info.S));
//                totalSize += info.S;
//                string sourcePath = string.Concat(System.Environment.CurrentDirectory
//                    , Path.DirectorySeparatorChar
//                    , StreamingAssetsFolder
//                    , Path.DirectorySeparatorChar
//                    , info.N
//                    , AbHelp.AbFileExt);
//                string targetPath = string.Concat(packageNamePath
//                    , Path.DirectorySeparatorChar
//                    , info.N
//                    , AbHelp.AbFileExt);
//                string[] paths = info.N.Split(new char[] { '/'});
//                if (paths.Length > 1)
//                {
//                    string tmpPath = packageNamePath;
//                    for (int j = 0; j < paths.Length - 1; j++)
//                    {
//                        tmpPath = string.Concat(tmpPath, Path.DirectorySeparatorChar, paths[j]);
//                        DirectoryInfo tmpDirectoryInfo = new DirectoryInfo(tmpPath);
//                        if(!tmpDirectoryInfo.Exists)
//                        {
//                            tmpDirectoryInfo.Create();
//                            getSshTask(sshTaskList, packagePath, tmpPath);
//                        }
//                    }
//                }
//                getSshTask(sshTaskList, packagePath, targetPath);
//                File.Copy(sourcePath, targetPath, true);
//            }

//            ABConfigInfo configInfo = JsonUtility.FromJson<ABConfigInfo>(File.ReadAllText(confInfoPath));

//            ABConfig config = JsonUtility.FromJson<ABConfig>(File.ReadAllText(confPath));

//            //string confStrPath = string.Concat(packageNamePath
//            //    , Path.DirectorySeparatorChar
//            //    , AbHelp.AbConfigStrName);
//            //string dat = JsonUtility.ToJson(config, true);
//            //FileEncryption.WriteAllBytes(confStrPath
//            //    , System.Text.Encoding.UTF8.GetBytes(dat));

//            //string confInfoStrPath = string.Concat(packageNamePath
//            //    , Path.DirectorySeparatorChar
//            //    , AbHelp.AbConfigInfoStrName);
//            //string dat = JsonUtility.ToJson(configInfo, true);
//            //FileEncryption.WriteAllBytes(confInfoStrPath
//            //    , System.Text.Encoding.UTF8.GetBytes(dat));

//            string dat = JsonUtility.ToJson(config);
//            string configPath = string.Concat(packageNamePath
//                , Path.DirectorySeparatorChar
//                , AbHelp.AbConfigName);
//            FileEncryption.WriteAllBytes(configPath, System.Text.Encoding.UTF8.GetBytes(dat));
//            getSshTask(sshTaskList, packagePath, configPath);

//            dat = JsonUtility.ToJson(configInfo);
//            string configInfoPath = string.Concat(packageNamePath
//                , Path.DirectorySeparatorChar
//                , AbHelp.AbConfigInfoName);
//            FileEncryption.WriteAllBytes(configInfoPath, System.Text.Encoding.UTF8.GetBytes(dat));
//            getSshTask(sshTaskList, packagePath, configInfoPath);

//            hotfixConfig.UpdatePath = AbHelp.DefaultUpdatePath;
//            hotfixConfig.FileSite = string.Concat(AbHelp.DefaultFileSite, AbHelp.GetPlatformStr(), currentVer);

//            string LastBuildVersionFile = string.Concat(System.Environment.CurrentDirectory
//                , Path.DirectorySeparatorChar
//                , LastBuildVersionPath
//                ,LastBuildVersionName);

//            if (File.Exists(LastBuildVersionFile))
//            {
//                LastBuildVersion lastVersion = JsonUtility.FromJson<LastBuildVersion>(File.ReadAllText(LastBuildVersionFile, Encoding.UTF8));
//                hotfixConfig.MinVer = lastVersion.AbVer;
//            }
//            else
//            {
//                hotfixConfig.MinVer = currentVer;
//            }
            
//            //写入App版本号
//            var appConfigTextAsset = Resources.Load<TextAsset>("AppConfig");
//            if (appConfigTextAsset == null)
//            {
//                throw new FileNotFoundException("AppConfig.txt");
//            }

//            var appConfig = JsonUtility.FromJson<AppConfig>(appConfigTextAsset.text);
//            var timeDiff = DateTime.Now.Ticks - startPointOfBuild.Ticks;
//            var curPatchNumber = (int)((float)timeDiff / 10000 / 1000 / 600);
//            appConfig.appVersion.Patch = curPatchNumber.ToString();
//            string version = appConfig.appVersion.GetBaseVersion();
//            hotfixConfig.AppVerison = version;

//            dat = JsonUtility.ToJson(hotfixConfig);
//            string hotfixConfigPath = string.Concat(System.Environment.CurrentDirectory
//             , Path.DirectorySeparatorChar
//             , PackagePath
//             , Path.DirectorySeparatorChar
//             , AbHelp.HotfixInfoName);
//            FileEncryption.WriteAllBytes(hotfixConfigPath, System.Text.Encoding.UTF8.GetBytes(dat));
//            getSshTask(sshTaskList, packagePath, hotfixConfigPath);

            
//            sb.AppendLine("Total size:" + AbHelp.GetSizeStr(totalSize));
//            Debug.LogWarning(sb.ToString());

//            if (uploadToServer)
//            {
//                EditorUtility.DisplayCancelableProgressBar("AB系统工作中...", "正在上传补丁包...", 0.7f);

//                WinScpSFTPUploadUtility.Start(packageNamePath, sshTaskList, currentVer);
//            }
//        }
//        else
//        {
//            Debug.LogError("ABMenu.makePackage read config fali!");
//        }
//    }

//    public static void DisplayUploadProcess(float process, string file)
//    {
//        UnityEditor.EditorUtility.DisplayCancelableProgressBar("AB系统工作中...", string.Concat("上传进度", process, "%当前文件：", file), 0.7f + ((process / 1000) * 3));
//    }
    
//    /// <summary>
//    /// 获取上传任务
//    /// </summary>
//    /// <param name="folderList"></param>
//    /// <param name="path"></param>
//    /// <param name="allPath"></param>
//    private static void getSshTask(List<SshTask> folderList, string path, string allPath)
//    {
//        string task = allPath.Substring(path.Length + 1);
//        string source = string.Concat(path, Path.DirectorySeparatorChar, task);
//        string target = string.Concat(WinSCPConfig.GetInstance().serverDesDir, AbHelp.GetPlatformStr(), task.Replace(Path.DirectorySeparatorChar, '/'));
//        folderList.Add(new SshTask(source, target));
//    }

//    /// <summary>
//    /// 删除文件
//    /// </summary>
//    /// <param name="list"></param>
//    public static void DeleteFile(List<FileInfo> list)
//    {
//        for (int i = 0; i < list.Count; i++)
//        {
//            FileInfo tmpFile = list[i];
//            if (tmpFile.Exists)
//            {
//                FileInfo metaFile = new FileInfo(string.Concat(tmpFile.FullName, AbHelp.FileExt[0]));
//                if (metaFile.Exists)
//                {
//                    metaFile.Attributes = FileAttributes.Normal;
//                    metaFile.Delete();
//                }
//                tmpFile.Delete();
//            }
//            tmpFile = null;
//        }
//        list.Clear();
//    }
//    //private static void copyDirectory(string srcdir, string desdir)
//    //{
//    //    string folderName = srcdir.Substring(srcdir.LastIndexOf(Path.DirectorySeparatorChar) + 1);
//    //    string desfolderdir = string.Concat(desdir, Path.DirectorySeparatorChar, folderName);
//    //    if (desdir.LastIndexOf(Path.DirectorySeparatorChar) == (desdir.Length - 1))
//    //    {
//    //        desfolderdir = desdir + folderName;
//    //    }
//    //    string[] filenames = Directory.GetFileSystemEntries(srcdir);
//    //    foreach (string file in filenames)
//    //    {
//    //        if (Directory.Exists(file))
//    //        {
//    //            string currentdir = string.Concat(desfolderdir, Path.DirectorySeparatorChar,  file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1));
//    //            if (!Directory.Exists(currentdir))
//    //            {
//    //                Directory.CreateDirectory(currentdir);
//    //            }
//    //            copyDirectory(file, desfolderdir);
//    //        }
//    //        else
//    //        {
//    //            if(!file.EndsWith(".manifest"))
//    //            {
//    //                string srcfileName = file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);
//    //                srcfileName = string.Concat(desfolderdir, Path.DirectorySeparatorChar, srcfileName);
//    //                if (!Directory.Exists(desfolderdir))
//    //                {
//    //                    Directory.CreateDirectory(desfolderdir);
//    //                }
//    //                if (srcfileName.LastIndexOf(".") == -1)
//    //                {
//    //                    srcfileName = string.Concat(srcfileName, AbHelp.AbFileExt);
//    //                }
//    //                File.Copy(file, srcfileName, true);
//    //            }
//    //        }
//    //    }
//    //}
//}
