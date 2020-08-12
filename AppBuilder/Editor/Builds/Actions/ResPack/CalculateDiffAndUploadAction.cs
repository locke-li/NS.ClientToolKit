///***************************************************************

// *  类名称：        BuildGameResourceAction

// *  描述：			计算差异并上传到远端服务器

// *  作者：          Chico(wuyuanbing)

// *  创建时间：      2020/4/26 11:03:58

// *  最后修改人：

// *  版权所有 （C）:   diandiangames

//***************************************************************/


//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using CenturyGame.AssetBundleManager.Editor.Builds.Core;
//using CenturyGame.AssetBundleManager.Runtime;
//using CenturyGame.Editor.UploadUtilitis;
//using CenturyGame.Editor.UploadUtilitis.WinSCP;
//using UnityEditor;
//using UnityEngine;
//using Version = CenturyGame.AssetBundleManager.Runtime.Version;
//#if DEBUG_FILE_CRYPTION
//using FileEncryption = CenturyGame.AssetBundleManager.Runtime.Core.IO.File;
//#else
//using FileEncryption = System.IO.File;
//#endif

//namespace CenturyGame.AssetBundleManager.Editor.Builds.Actions.ResPack
//{
//    public class CalculateDiffAndUploadAction : BaseBuildFilterAction
//    {
//        //--------------------------------------------------------------
//        #region Fields
//        //--------------------------------------------------------------

//        #endregion

//        //--------------------------------------------------------------
//        #region Properties & Events
//        //--------------------------------------------------------------

//        #endregion

//        //--------------------------------------------------------------
//        #region Creation & Cleanup
//        //--------------------------------------------------------------

//        #endregion

//        //--------------------------------------------------------------
//        #region Methods
//        //--------------------------------------------------------------

//        #endregion
//        public override void Execute(IFilter filter, IPipelineInput input)
//        {
//            this.State = ActionState.Normal;
//            mergerFile();
//            this.State = ActionState.Completed;
//        }
//        private List<FileDesc> CalculateVersionDiffFormBasic()
//        {
//            var appBuildContext = Context.GetAppBuildContext();
//            string lastVersionPath = appBuildContext.GetLastVerisonPath();
           
//            List<FileDesc> packageFileList = new List<FileDesc>();
//            if (File.Exists(lastVersionPath))
//            {
//                LastBuildVersion target = JsonUtility.FromJson<LastBuildVersion>(File.ReadAllText(lastVersionPath, Encoding.UTF8));
//                string confInfoStrPath = appBuildContext.GetVersionManifestPath();
//                Dictionary<string, ABTableItemInfoClient> targetList = new Dictionary<string, ABTableItemInfoClient>();
//                if (File.Exists(confInfoStrPath))
//                {
//                    VersionManifest configInfo = JsonUtility.FromJson<VersionManifest>(File.ReadAllText(confInfoStrPath, Encoding.UTF8));
//                    for (int i = 0; i < target.Info.List.Length; i++)
//                    {
//                        ABTableItemInfoClient info = target.Info.List[i];
//                        targetList[info.N] = info;
//                    }
//                    for (int i = 0; i < configInfo.Datas.Count; i++)
//                    {
//                        var info = configInfo.Datas[i];
//                        ABTableItemInfoClient client = null;
//                        if (targetList.ContainsKey(info.N))
//                        {
//                            client = targetList[info.N];
//                            if (client.H == info.H)
//                            {
//                                if (client.R)
//                                {
//                                    packageFileList.Add(info);
//                                }
//                            }
//                            else
//                            {
//                                packageFileList.Add(info);
//                                client.R = true;
//                            }
//                        }
//                        else
//                        {
//                            client = new ABTableItemInfoClient(info);
//                            client.R = true;
//                            targetList[info.N] = client;
//                            packageFileList.Add(info);
//                        }
//                    }
//                }

//                if (packageFileList.Count > 0)
//                {
//                    target.Info.List = targetList.Values.ToArray();
//                    var dat = JsonUtility.ToJson(target, true);
//                    FileEncryption.WriteAllBytes(lastVersionPath, System.Text.Encoding.UTF8.GetBytes(dat));
//                    Debug.Log("hotfix save LastBuildVersion:" + lastVersionPath + ", file cout:" + packageFileList.Count);
//                }
//            }
//            else//如果没有此文件，相当于重新做了第一个版本
//            {
//                Debug.LogWarning("No last build version.");
//            }

//            return packageFileList;
//        }

//        private void mergerFile()
//        {
//            var appBuildContext = Context.GetAppBuildContext();
//            FileInfo targetFile = new FileInfo(string.Concat(System.Environment.CurrentDirectory
//                    , Path.DirectorySeparatorChar
//                    , appBuildContext.LastBuildVersionPath
//                    , appBuildContext.LastBuildVersionName));
//            List<FileDesc> packageFileList = new List<FileDesc>();
//            LastBuildVersion target = null;
//            if (targetFile.Exists)
//            {
//                target = JsonUtility.FromJson<LastBuildVersion>(File.ReadAllText(targetFile.FullName, Encoding.UTF8));
//                string confInfoStrPath = string.Concat(System.Environment.CurrentDirectory
//                    , Path.DirectorySeparatorChar
//                    , appBuildContext.StreamingAssetsFolder
//                    , Path.DirectorySeparatorChar
//                    , AbHelp.AbConfigInfoName);
//                Dictionary<string, ABTableItemInfoClient> targetList = new Dictionary<string, ABTableItemInfoClient>();

//                if (File.Exists(confInfoStrPath))
//                {
//                    VersionManifest configInfo = JsonUtility.FromJson<VersionManifest>(File.ReadAllText(confInfoStrPath, Encoding.UTF8));
//                    for (int i = 0; i < target.Info.List.Length; i++)
//                    {
//                        ABTableItemInfoClient info = target.Info.List[i];
//                        targetList[info.N] = info;
//                    }
//                    for (int i = 0; i < configInfo.Datas.Count; i++)
//                    {
//                        var info = configInfo.Datas[i];
//                        ABTableItemInfoClient client = null;
//                        if (targetList.ContainsKey(info.N))
//                        {
//                            client = targetList[info.N];
//                            if (client.H == info.H)
//                            {
//                                if (client.R)
//                                {
//                                    packageFileList.Add(info);
//                                }
//                            }
//                            else
//                            {
//                                packageFileList.Add(info);
//                                client.R = true;
//                            }
//                        }
//                        else
//                        {
//                            client = new ABTableItemInfoClient(info);
//                            client.R = true;
//                            targetList[info.N] = client;
//                            packageFileList.Add(info);
//                        }
//                    }
//                }

//                if (packageFileList.Count > 0)
//                {
//                    target.Info.List = targetList.Values.ToArray();
//                    var dat = JsonUtility.ToJson(target, true);
//                    FileEncryption.WriteAllBytes(targetFile.FullName, System.Text.Encoding.UTF8.GetBytes(dat));
//                    Debug.Log("hotfix save LastBuildVersion:" + targetFile.FullName + ", file cout:" + packageFileList.Count);
//                }
//            }
//            else//如果没有此文件，相当于重新做了第一个版本
//            {
//                Debug.LogWarning("No last build version.");
//            }
//            makePackage(packageFileList,target);
//        }

//        static DateTime startPointOfBuild = new DateTime(2020, 4, 23, 18, 0, 0, 0);


       

//        private static void makePackage(List<FileDesc> list, LastBuildVersion target)
//        {
//            var appBuildContext = Context.GetAppBuildContext();

//            string packagePath = string.Concat(System.Environment.CurrentDirectory
//                , Path.DirectorySeparatorChar
//                , appBuildContext.PackagePath);

           

//            HotFixConfig hotfixConfig = new HotFixConfig();
//            List<UploadTask> sshTaskList = new List<UploadTask>();
//            StringBuilder sb = new StringBuilder();

//            //string confPath = string.Concat(System.Environment.CurrentDirectory
//            //    , Path.DirectorySeparatorChar
//            //    , appBuildContext.StreamingAssetsFolder
//            //    , Path.DirectorySeparatorChar
//            //    , AbHelp.AbConfigName);
//            //string confInfoPath = string.Concat(System.Environment.CurrentDirectory
//            //    , Path.DirectorySeparatorChar
//            //    , appBuildContext.StreamingAssetsFolder
//            //    , Path.DirectorySeparatorChar
//            //    , AbHelp.AbConfigInfoName);

//            //if (File.Exists(confPath) && File.Exists(confInfoPath))
//            if (true)
//            {
//                var appVersion = AppBuildConfig.GetAppBuildConfigInst().targetAppVersion;
//                Version baseVersion = null;
//                Version curVersion = null;
//                if (target != null)
//                {
//                    baseVersion = new Version(target.Version);
//                }

//                if (baseVersion == null)
//                {
//                    string versionStr = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}";
//                    curVersion = new Version(versionStr);
//                }
//                else
//                {
//                    var targetVersinStr = appVersion.GetBaseVersion();
//                    Version targetVersion = new Version(targetVersinStr);

//                    var result = targetVersion.CompareTo(baseVersion);

//                    switch (result)
//                    {
//                        case Version.VersionCompareResult.HigherForMajor:
//                            targetVersinStr = $"{appVersion.Major}.0.0";
//                            Debug.LogError("Generate a new major version !");
//                            break;
//                        case Version.VersionCompareResult.HigherForMinor:
//                            targetVersinStr = $"{appVersion.Major}.{appVersion.Minor}.0";
//                            Debug.LogError("Generate a new minor version !");
//                            break;
//                        case Version.VersionCompareResult.HigherForPatch:
//                            targetVersinStr = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}";
//                            Debug.LogError("Generate a new minor version !");
//                            break;
//                        case Version.VersionCompareResult.LowerForPatch:
//                            targetVersinStr = $"{appVersion.Major}.{appVersion.Minor}.{baseVersion.PatchNum}";
//                            Debug.LogError("Increment the old patch number!");
//                            break;
//                    }

//                    if (string.IsNullOrEmpty(targetVersinStr))
//                    {
//                        throw new Exception($"Generate version failure ! result : {result} !");
//                    }

//                    curVersion = new Version(targetVersinStr);
//                }

//                if (list.Count > 0)
//                {
//                    curVersion.IncrementOneForPatch();
//                }

//                string version = curVersion.GetVersionString();
//                appBuildContext.VersinManifest.Ver = version;

//                string confInfoPath = string.Concat(System.Environment.CurrentDirectory
//                    , Path.DirectorySeparatorChar
//                    , appBuildContext.StreamingAssetsFolder
//                    , Path.DirectorySeparatorChar
//                    , AbHelp.AbConfigInfoName);
//                var dat = JsonUtility.ToJson(appBuildContext.VersinManifest);
//                //保存文件清单
//                FileEncryption.WriteAllBytes(confInfoPath, System.Text.Encoding.UTF8.GetBytes(dat));

//                //Create target pachage folder
//                string sourceFolderPath = string.Concat(packagePath
//                    , Path.DirectorySeparatorChar,
//                    curVersion.GetVersionFolderString(), Path.DirectorySeparatorChar, AbHelp.GetPlatformNameNoSlash());
//                sourceFolderPath = EditorUtils.OptimazePath(sourceFolderPath);
//                DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(sourceFolderPath);
//                if (sourceDirectoryInfo.Exists)
//                    sourceDirectoryInfo.Delete(true);
//                sourceDirectoryInfo.Create();

//                int totalSize = 0;
//                string filesSourcePath = string.Concat(sourceFolderPath, Path.DirectorySeparatorChar,
//                    curVersion.GetVersionFolderString());
//                DirectoryInfo filesSourceDirectoryInfo = new DirectoryInfo(filesSourcePath);
//                if (filesSourceDirectoryInfo.Exists)
//                    filesSourceDirectoryInfo.Delete(true);
//                filesSourceDirectoryInfo.Create();

//                for (int i = 0; i < list.Count; i++)
//                {
//                    var info = list[i];
//                    sb.AppendLine(info.N + ":" + AbHelp.GetSizeStr(info.S));
//                    totalSize += info.S;

//                    string sourcePath = string.Concat(System.Environment.CurrentDirectory
//                        , Path.DirectorySeparatorChar
//                        , appBuildContext.StreamingAssetsFolder
//                        , Path.DirectorySeparatorChar
//                        , info.N);
                    
//                    string targetPath = string.Concat(filesSourcePath
//                        , Path.DirectorySeparatorChar
//                        , info.N);
//                    string[] paths = info.N.Split(new char[] { '/' });
//                    if (paths.Length > 1)
//                    {
//                        string tmpPath = filesSourcePath;
//                        for (int j = 0; j < paths.Length - 1; j++)
//                        {
//                            tmpPath = string.Concat(tmpPath, Path.DirectorySeparatorChar, paths[j]);
//                            DirectoryInfo tmpDirectoryInfo = new DirectoryInfo(tmpPath);
//                            if (!tmpDirectoryInfo.Exists)
//                            {
//                                tmpDirectoryInfo.Create();
//                            }
//                        }
//                    }
//                    //将差异文件拷贝到Package目录下，以便上传
//                    File.Copy(sourcePath, targetPath, true);
//                }

//                // 将文件版本信息清单拷贝到Package目录下
//                VersionManifest versionManifest = JsonUtility.FromJson<VersionManifest>(File.ReadAllText(confInfoPath));
//                dat = JsonUtility.ToJson(versionManifest);
//                string configInfoPath = string.Concat(filesSourcePath
//                    , Path.DirectorySeparatorChar
//                    , AbHelp.AbConfigInfoName);
//                FileEncryption.WriteAllBytes(configInfoPath, System.Text.Encoding.UTF8.GetBytes(dat));

//                var appBuildConfig = AppBuildConfig.GetAppBuildConfigInst();

//#if UNITY_ANDROID
//                hotfixConfig.UpdatePath = appBuildConfig.androidAppUpdateUrl;
//#elif UNITY_IOS
//                hotfixConfig.UpdatePath = appBuildConfig.iosAppUpdateUrl;
//#endif
//                hotfixConfig.FileSite = string.Concat(AbHelp.DefaultFileSite, AbHelp.GetPlatformStr(), curVersion.GetVersionFolderString());
//                string LastBuildVersionFile = string.Concat(System.Environment.CurrentDirectory
//                    , Path.DirectorySeparatorChar
//                    , appBuildContext.LastBuildVersionPath
//                    , appBuildContext.LastBuildVersionName);

//                if (File.Exists(LastBuildVersionFile))
//                {
//                    LastBuildVersion lastVersion = JsonUtility.FromJson<LastBuildVersion>(File.ReadAllText(LastBuildVersionFile, Encoding.UTF8));
//                    hotfixConfig.MinVer = lastVersion.Version;
//                }
//                else
//                {
//                    hotfixConfig.MinVer = version;
//                }

//                hotfixConfig.AppVerison = version;


//                dat = JsonUtility.ToJson(hotfixConfig);
//                string hotfixConfigPath = string.Concat(sourceFolderPath
//                    , Path.DirectorySeparatorChar
//                    , AbHelp.HotfixInfoName);
//                FileEncryption.WriteAllBytes(hotfixConfigPath, System.Text.Encoding.UTF8.GetBytes(dat));

//                sb.AppendLine("Total size:" + AbHelp.GetSizeStr(totalSize));
//                Debug.LogWarning(sb.ToString());

//                WinScpSFTPUploadUtility.Start(sourceFolderPath);

//                AssetDatabase.Refresh();
//            }
//            else
//            {
//                Debug.LogError("ABMenu.makePackage read config fali!");
//            }
//        }


//    }
//}
