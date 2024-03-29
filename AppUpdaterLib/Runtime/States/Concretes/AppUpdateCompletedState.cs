﻿/***************************************************************

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

            Context.SaveAppRevision();

            Context.ProgressData.Progress = 1;
        }

        public override void Execute(AppUpdaterFsmOwner entity)
        {
            base.Execute(entity);

            this.OnAppUpdateCompleted();
        }


        private void OnAppUpdateCompleted()
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
