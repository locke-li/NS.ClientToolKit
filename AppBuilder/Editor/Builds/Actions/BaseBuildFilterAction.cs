/***************************************************************

 *  类名称：        BaseBuildFilterAction

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/11 16:49:32

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using CenturyGame.AppBuilder.Editor.Builds.Contexts;
using CenturyGame.Core.Pipeline;
using CenturyGame.LoggerModule.Runtime;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions
{
    public class BaseBuildFilterAction : BaseFilterAction 
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------
        protected override ILogger Logger
        {
            get
            {
                if (s_mlogger == null)
                    s_mlogger = LoggerManager.GetLogger(this.GetType().Name);
                return s_mlogger;
            }
        }
        private static ILogger s_mlogger;

        public override IPipelineContext Context
        {
            get => base.Context;
            set
            {
                base.Context = value;
                this.mAppBuildContext = value as AppBuildContext;
            }
        }

        private AppBuildContext mAppBuildContext;
        public AppBuildContext AppBuildContext => mAppBuildContext;

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        public override bool Test(IFilter filter, IPipelineInput input)
        {
            return false;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
        }

        #endregion
    }
}
