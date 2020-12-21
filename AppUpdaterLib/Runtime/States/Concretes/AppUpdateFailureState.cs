/***************************************************************

 *  类名称：          HotFixFailureState

 *  描述：            热更失败

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/20 21:29:01

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using CenturyGame.AppUpdaterLib.Runtime.Helps;

namespace CenturyGame.AppUpdaterLib.Runtime.States.Concretes
{
    internal class AppUpdateFailureState : BaseAppUpdaterFunctionalState
    {
        public override void Enter(AppUpdaterFsmOwner entity, params object[] args)
        {
            base.Enter(entity, args);

            var errorType = Context.ErrorType;

            if (errorType == AppUpdaterErrorType.None)
                throw new InvalidOperationException(errorType.ToString());
            this.Target.OnErrorCallback(errorType, ErrorTypeHelper.GetErrorString(errorType));
            Context.AppendInfo("App updater failure");
        }
    }
}
