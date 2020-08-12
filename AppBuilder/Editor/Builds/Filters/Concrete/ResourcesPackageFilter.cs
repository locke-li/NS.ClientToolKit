/***************************************************************

 *  类名称：        ResourcesPackageFilter

 *  描述：			资源打包部分，包括资源的扫描，输出清单，生成资源版本号等相关操作

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/24 20:57:00

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack;
using CenturyGame.Core.Pipeline;
using CenturyGame.LoggerModule.Runtime;

namespace CenturyGame.AppBuilder.Editor.Builds.Filters.Concrete
{
    public class ResourcesPackageFilter : QueueActionsPipelineFilter
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

        public ResourcesPackageFilter()
        {
        }

        public ResourcesPackageFilter(bool autoAddActions) : base(autoAddActions)
        {
        }

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        public override void OnAutoAddActions()
        {
            this.pSequence.Enqueue(new AssetBundleFilesPackAction());
            this.pSequence.Enqueue(new FileVersionManifestGenerateAction());
        }


        #endregion

    }
}
