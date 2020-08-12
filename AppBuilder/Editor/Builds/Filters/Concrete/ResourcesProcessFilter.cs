/***************************************************************

 *  类名称：        ResourcesProcessFilter

 *  描述：		    对需要热更的资源做处理，以供资源打包Filter使用

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/24 20:55:58

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using CenturyGame.AppBuilder.Editor.Builds.Actions.ResProcess;
using CenturyGame.Core.Pipeline;
using CenturyGame.LoggerModule.Runtime;

namespace CenturyGame.AppBuilder.Editor.Builds.Filters.Concrete
{
    public class ResourcesProcessFilter : QueueActionsPipelineFilter
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

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------
        public ResourcesProcessFilter()
        {
        }

        public ResourcesProcessFilter(bool autoAddActions) : base(autoAddActions)
        {
        }

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        public override void OnAutoAddActions()
        {
            this.pSequence.Enqueue(new TestOtherFilesExportAction());
            this.pSequence.Enqueue(new AssetBundleAction());
        }

        #endregion

    }
}
