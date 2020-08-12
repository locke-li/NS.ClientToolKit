///***************************************************************

// *  类名称：        DeleteBasicBuildVersionInfoAction

// *  描述：				

// *  作者：          Chico(wuyuanbing)

// *  创建时间：      2020/4/26 10:16:21

// *  最后修改人：

// *  版权所有 （C）:   diandiangames

//***************************************************************/


//using System.IO;
//using System.Text;
//using CenturyGame.Core.Pipeline;
//using UnityEngine;

//namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResProcess
//{
//    public class DeleteBasicBuildVersionInfoAction : BaseBuildFilterAction
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

//        public override void Execute(IFilter filter, IPipelineInput input)
//        {
//            this.State = ActionState.Normal;
//            this.deleteBulidVersionInfo(input);
//            this.State = ActionState.Completed;
//        }

        
//        private void deleteBulidVersionInfo(IPipelineInput input)
//        {
//            var appBuildContext = Context.GetAppBuildContext();

//            Debug.Log("ABMenu.deleteBulidVersionInfo...");
//            FileInfo targetFile = new FileInfo(appBuildContext.LastBuildAppInfoFilePath);
//            if (targetFile.Exists)
//            {
//                LastBuildVersion lastVersion = JsonUtility.FromJson<LastBuildVersion>(File.ReadAllText(targetFile.FullName, Encoding.UTF8));
//                if (!string.IsNullOrEmpty(lastVersion.Version))
//                {
//                    //appBuildContext.lastBuildVer = lastVersion.AbVer;
//                    //string movePath = string.Concat(System.Environment.CurrentDirectory
//                    //    , Path.DirectorySeparatorChar
//                    //    , appBuildContext.PackagePath
//                    //    , Path.DirectorySeparatorChar
//                    //    , appBuildContext.lastBuildVer
//                    //    , Path.DirectorySeparatorChar);
//                    //if (Directory.Exists(movePath))
//                    //{
//                    //    targetFile.CopyTo(string.Concat(movePath, appBuildContext.LastBuildVersionName), true);
//                    //    Debug.Log("ABMenu.deleteBulidVersionInfo ok");
//                    //}
//                }
//                targetFile.Attributes = FileAttributes.Normal;
//                targetFile.Delete();
//                FileInfo targetFileMeta = new FileInfo(string.Concat(targetFile.FullName, AbHelp.FileExt[0]));
//                if (targetFileMeta.Exists)
//                {
//                    targetFileMeta.Attributes = FileAttributes.Normal;
//                    targetFileMeta.Delete();
//                }
//            }
//            else
//            {
//                Debug.Log("ABMenu.deleteBulidVersionInfo first build!");
//            }
//        }
        

//        #endregion

//    }
//}
