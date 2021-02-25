/***************************************************************

 *  类名称：          HotFixIdleState

 *  描述：            热更初始状态

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/20 18:23:46

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.AppUpdaterLib.Runtime.Managers;
using CenturyGame.Core.FSM;

namespace CenturyGame.AppUpdaterLib.Runtime.States.Concretes
{
    internal sealed class AppUpdateInitState : BaseAppUpdaterFunctionalState
    {
        public override void Execute(AppUpdaterFsmOwner entity)
        {
            base.Execute(entity);

            if (!AppUpdaterHints.Instance.ManualPerformAppUpdate)
            {
                this.PerformAppUpdate();
            }
        }

        public override bool OnMessage(AppUpdaterFsmOwner entity, in IRoutedEventArgs eventArgs)
        {
            var eventType = (AppUpdaterInnerEventType)eventArgs.EventType;

            switch (eventType)
            {
                case AppUpdaterInnerEventType.PerformAppUpdate:
                    this.PerformAppUpdate();
                    return true;
            }

            return base.OnMessage(entity, in eventArgs);
        }

        private void PerformAppUpdate()
        {
            this.Target.State = AppUpdaterFsmOwner.AppUpdaterState.Runing;
            var appUpdaterConfig = AppUpdaterConfigManager.AppUpdaterConfig;
            if (appUpdaterConfig.skipAppUpdater)
            {
                Context.AppendInfo("Skip app updater !");
                this.Target.ChangeState<AppUpdateFinalState>();
            }
            else
            {
                Context.AppendInfo("AppUpdater start working!");
                this.Target.ChangeState<AppUpdateGetLighthouseConfigState>();
            }
        }

    }
}
