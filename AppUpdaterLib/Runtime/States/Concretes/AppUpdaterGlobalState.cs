/***************************************************************

 *  类名称：          AppUpdaterGlobalState

 *  描述：            热更全局状态，处理全局跳转相关

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/20 18:36:24

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


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
                //case AppUpdaterInnerEventType.OnApplicationFocus:
                //    return this.AppFocusHandleCallback(in eventArgs);
            }
            return base.OnMessage(entity, in eventArgs);
        }


        private void CheckResUpdateWasDone()
        {
            Context.ResUpdateTarget.CurrentResVersionIdx++;
            if (Context.ResUpdateTarget.CurrentResVersionIdx == Context.ResUpdateTarget.ResVersions.Length)// Resource update was Done!
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

        //private bool AppFocusHandleCallback(in IRoutedEventArgs arg)
        //{
        //    return false;
        //}
    }
}
