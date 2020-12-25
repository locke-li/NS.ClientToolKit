/***************************************************************

 *  类名称：        AppUpdaterDeferredDownloadInitState

 *  描述：		    延迟下载一系列文件

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/12/22 18:12:21

 *  最后修改人：    将可选资源同步到本地

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.AppUpdaterLib.Runtime.States;
using CenturyGame.Core.FSM;

namespace CenturyGame.AppUpdaterLib.Runtime.States.Concretes
{
    internal class AppUpdaterDeferredDownloadInitState : BaseAppUpdaterFunctionalState
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        public override void Enter(AppUpdaterFsmOwner entity, params object[] args)
        {
            this.Setup();
        }

        void Setup()
        {
        }

        public override void Execute(AppUpdaterFsmOwner entity)
        {
            base.Execute(entity);
        }


        public override void Exit(AppUpdaterFsmOwner entity)
        {
            base.Exit(entity);
        }

        #endregion

    }
}
