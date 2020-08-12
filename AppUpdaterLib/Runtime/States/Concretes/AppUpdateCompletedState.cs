/***************************************************************

 *  类名称：          HotFixCompletedState

 *  描述：            热更完成

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/20 20:13:50

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System.Text;

namespace CenturyGame.AppUpdaterLib.Runtime.States.Concretes
{
    internal class AppUpdateCompletedState : BaseAppUpdaterFunctionalState
    {

        public override void Enter(AppUpdaterFsmOwner entity, params object[] args)
        {
            base.Enter(entity, args);

            Context.ProgressData.Progress = 1;
           
        }

        /*
        private void ReadFileList()
        {
            string path = AssetsFileSystem.GetWritePath(AssetsFileSystem.AbConfigName);
            if (File.Exists(path))//编辑器下有可能不会有这个文件
            {
                Debug.Log(string.Concat("load rw path:", path));

#if UNITY_EDITOR
                this.mLogSb.AppendLine(string.Concat("load rw path:", path));
                this.mLogSb.AppendLine("Start read local ab config!");
#endif

                var bytes = File.ReadAllBytes(path);
                this.ReadAbConfigCompleted(bytes);
            }
            else
            {
                path = AssetsFileSystem.GetStreamingAssetsPath(AssetsFileSystem.AbConfigName, null, false);
                Debug.Log(string.Concat("load buildin path :", path));

#if UNITY_EDITOR
                this.mLogSb.AppendLine(string.Concat("load buildin path :", path));
                this.mLogSb.AppendLine("Start read buildin config!");
#endif

                this.Target.Request.Load(path, readConfigCallback);
            }
        }

        private void readConfigCallback(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
#if UNITY_EDITOR
                this.mLogSb.AppendLine(string.Concat("readConfigCallback failure !"));
#endif
                Context.ErrorCode = ErrorCodeConst.READ_ASSETBUNDLE_CONFIG;

                this.Target.ChangeState<HotFixFailureState>();
            }
            else
            {
                this.ReadAbConfigCompleted(data);
            }
        }

        private void ReadAbConfigCompleted(byte[] bytes)
        {
#if UNITY_EDITOR
            this.mLogSb.AppendLine(string.Concat("ReadAbConfigCompleted completed !"));
#endif
            AssetsFileSystem.AbConfig = JsonUtility.FromJson<ResManifest>(System.Text.Encoding.UTF8.GetString(bytes));
            this.Target.ChangeState<HotFixFinalState>();
        }
        */


        public override void Execute(AppUpdaterFsmOwner entity)
        {
            base.Execute(entity);

            this.OnHotfixCompleted();
        }


        private void OnHotfixCompleted()
        {
            this.Target.ChangeState<AppUpdateFinalState>();

            if (Context.ErrorType == AppUpdaterErrorType.None)
            {
                Context.AppendInfo("Resource update completed!");
            }
            else
            {
                Context.AppendInfo("Resource update failure!");
            }

        }

        public override void Exit(AppUpdaterFsmOwner entity)
        {
            base.Exit(entity);
#if UNITY_EDITOR
            this.mSb.Clear();
            this.mLogSb.Clear();
#endif
        }

#if UNITY_EDITOR

        private StringBuilder mSb = new StringBuilder();
        private StringBuilder mLogSb = new StringBuilder();
        public override string ToString()
        {
            this.mSb.Length = 0;
            this.mSb.AppendLine("State : " + this.GetType().Name);
            this.mSb.AppendLine("Log : \n"+this.mLogSb.ToString());

            return this.mSb.ToString();
        }
#endif

    }
}
