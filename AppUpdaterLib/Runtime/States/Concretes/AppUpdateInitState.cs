/***************************************************************

 *  类名称：          HotFixIdleState

 *  描述：            热更初始状态

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/20 18:23:46

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

namespace CenturyGame.AppUpdaterLib.Runtime.States.Concretes
{
    internal sealed class AppUpdateInitState : BaseAppUpdaterFunctionalState
    {

        public override void Execute(AppUpdaterFsmOwner entity)
        {
            base.Execute(entity);

            if (Context.Config.skipAppUpdater)
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
