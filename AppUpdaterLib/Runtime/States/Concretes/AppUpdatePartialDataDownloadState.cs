/***************************************************************

 *  类名称：        AppUpdatePartialDataDownloadState

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2021/3/1 16:21:56

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenturyGame.AppUpdaterLib.Runtime.Managers;
using CenturyGame.Core.FSM;

// ReSharper disable once CheckNamespace
namespace CenturyGame.AppUpdaterLib.Runtime.States.Concretes
{
    sealed class AppUpdatePartialDataDownloadState : BaseAppUpdaterFunctionalState
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
            this.Target.State = AppUpdaterFsmOwner.AppUpdaterState.Runing;
            base.Enter(entity, args);
        }

        public override void Execute(AppUpdaterFsmOwner entity)
        {
            base.Execute(entity);
            StartUpdateRes();
        }

        private void StartUpdateRes()
        {
            Logger.Info("Start to check update resource !");
            VersionDesc info = new VersionDesc
            {
                Type = UpdateResourceType.NormalResource,
                LocalMd5 = AppVersionManager.AppInfo.unityDataResVersion,
                RemoteMd5 = AppVersionManager.AppInfo.unityDataResVersion,
            };

            Context.ResUpdateTarget.VersionDescs = new VersionDesc[]
            {
                info
            };

            Context.ResUpdateConfig.Mode = ResSyncMode.SUB_GROUP;
            Context.ResUpdateConfig.Filter = this.Target.FileUpdateRuleFilter;
            Context.ResUpdateTarget.CurrentResVersionIdx++;

            this.Target.ChangeState<AppUpdateDataResState>();
            IRoutedEventArgs arg = new RoutedEventArgs()
            {
                EventType = (int)AppUpdaterInnerEventType.PerformResUpdateOperation
            };
            this.Target.HandleMessage(in arg);
        }

        #endregion

    }
}
