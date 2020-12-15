/***************************************************************

 *  类名称：          HotFixFinalState

 *  描述：            热更新服务的终态

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/21 14:24:47

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using CenturyGame.Core.MessengerSystem;
using System.Threading;

namespace CenturyGame.AppUpdaterLib.Runtime.States.Concretes
{
    internal class AppUpdateFinalState : BaseAppUpdaterFunctionalState
    {
        public override void Enter(AppUpdaterFsmOwner entity, params object[] args)
        {
            base.Enter(entity, args);

            this.Target.OnCompletedCallback();
        }
    }
}
