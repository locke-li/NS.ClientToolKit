/**************************************************************
 *  类名称：          AppBuilderPrepareFilter
 *  描述：
 *  作者：            Chico(wuyuanbing)
 *  创建时间：        2020/12/7 14:36:27
 *  最后修改人：
 *  版权所有 （C）:   CenturyGames
 **************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenturyGame.Core.Pipeline;
using CenturyGame.LoggerModule.Runtime;

namespace CenturyGame.AppBuilder.Editor.Builds.Filters.Concrete
{
    class AppBuilderPrepareFilter : QueueActionsPipelineFilter
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

        #endregion


        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        #endregion
    }
}