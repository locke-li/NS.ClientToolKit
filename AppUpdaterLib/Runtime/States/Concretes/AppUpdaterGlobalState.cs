/***************************************************************

 *  类名称：          AppUpdaterGlobalState

 *  描述：            热更全局状态，处理全局跳转相关

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/20 18:36:24

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using CenturyGame.AppUpdaterLib.Runtime.Managers;
using CenturyGame.Core.FSM;

namespace CenturyGame.AppUpdaterLib.Runtime.States.Concretes
{
    internal sealed class AppUpdaterGlobalState : BaseAppUpdaterFunctionalState
    {

        public override bool OnMessage(AppUpdaterFsmOwner entity, in IRoutedEventArgs eventArgs)
        {
            var evtType = (AppUpdaterInnerEventType)eventArgs.EventType;
            Logger.Debug($"AppUpdater receive msg , type : {evtType.ToString()}");
            switch (evtType)
            {
                case AppUpdaterInnerEventType.OnCurrentResUpdateCompleted:
                    this.CheckResUpdateWasDone();
                    return true;
                case AppUpdaterInnerEventType.StartPerformResUpdateOperation:
                    this.CheckResUpdateWasDone();
                    return true;
                case AppUpdaterInnerEventType.StartPerformResPartialUpdateOperation:
                    if (AppUpdaterConfigManager.AppUpdaterConfig.skipAppUpdater)
                    {
                        Logger.Warn("You can't download partial data resources, because current runing mode will skip appupdate operation!");
                        this.Target.ChangeState<AppUpdateCompletedState>();
                    }
                    else
                    {
                        this.Target.ChangeState<AppUpdatePartialDataDownloadState>();
                    }
                    return true;
            }
            return base.OnMessage(entity, in eventArgs);
        }


        private void CheckResUpdateWasDone()
        {
            Context.ResUpdateTarget.CurrentResVersionIdx++;
            if (Context.IsUpdateCompleted())// Resource update was Done!
            {
                this.Target.ChangeState<AppUpdateCompletedState>();
            }
            else
            {
                this.PerformNextResUpdateOperation();
            }
        }

        private void PerformNextResUpdateOperation()
        {
            this.Target.ChangeState<AppUpdateDataResState>();
            IRoutedEventArgs arg = new RoutedEventArgs()
            {
                EventType = (int)AppUpdaterInnerEventType.PerformResUpdateOperation
            };
            this.Target.HandleMessage(in arg);
        }

    }
}
