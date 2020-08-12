/***************************************************************

 *  类名称：          BaseAppUpdaterFunctionalState

 *  描述：

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/21 15:19:40

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using CenturyGame.Core.FSM;
using CenturyGame.LoggerModule.Runtime;

namespace CenturyGame.AppUpdaterLib.Runtime.States
{
    internal abstract class BaseAppUpdaterFunctionalState : State<AppUpdaterFsmOwner>
    {
#if DEBUG_APP_UPDATER
        private string mStateDesc = null;
        public virtual string StateDesc
        {
            // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
            get { return mStateDesc ?? (mStateDesc = this.GetType().Name); }
        }
#endif
        public AppUpdaterContext Context => this.Target.Context;

        private ILogger mLogger = null;

        protected override ILogger Logger
        {
            // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
            get { return mLogger ?? (mLogger = LoggerManager.GetLogger(this.GetType().Name)); }
        }

        public override void Enter(AppUpdaterFsmOwner entity, params object[] args)
        {
#if DEBUG_APP_UPDATER
            Context.StateName = this.StateDesc;
#endif
            base.Enter(entity, args);
        }

    }
}
