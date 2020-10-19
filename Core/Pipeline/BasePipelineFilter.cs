/***************************************************************

 *  类名称：        BasePipelineFilter

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/24 20:32:17

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;

namespace CenturyGame.Core.Pipeline
{
    public abstract class BasePipelineFilter : IFilter
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        protected abstract ILogger Logger { get; }

        protected IPipelineFilterAction pAction = null;

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        public bool Enabled { set; get; } = true;

        public IFilter NextFilter { private set; get; }

        public IPipelineProcessor Processor { set ; get; }

        public FilterState State { private set; get; } = FilterState.Normal;

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------
        public void Connect(IFilter nextFilter)
        {
            this.NextFilter = nextFilter;
        }

        public virtual bool Test(IPipelineInput input)
        {
            return pAction.Test(this,input);
        }

        public virtual void Execute(IPipelineInput input)
        {
            this.State = FilterState.Normal;
            if (this.pAction != null)
            {
                try
                {
                    this.pAction.State = ActionState.Normal;

                    this.pAction.BeforeExcute(this, input);

                    this.pAction.Execute(this, input);

                    this.pAction.EndExcute(this, input);

                    if (this.pAction.State == ActionState.Error)
                    {
                        this.State = FilterState.Error;
                    }
                    else
                    {
                        this.State = FilterState.Completed;
                    }
                }
                catch (Exception ex)
                {
                    var context = this.Processor.Context;
                    context.AppendErrorLog("Error message : " + ex.Message + " stackTrace : " + ex.StackTrace);
                    this.State = FilterState.Error;
                }
               
            }
            else
            {
                this.State = FilterState.Completed;
            }
        }


        public virtual void OnPreProcess()
        {
            Logger?.Debug($"Begin perform filter :{this.GetType().Name}");
        }


        public virtual void OnPostProcess()
        {
            Logger?.Debug($"End perform filter :{this.GetType().Name}");
        }

        public virtual void SetAction(IPipelineFilterAction action)
        {
            this.pAction = action;
            this.pAction.Context = this.Processor.Context;
        }

        #endregion

    }
}
