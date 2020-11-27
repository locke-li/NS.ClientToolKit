/***************************************************************

 *  类名称：        BasePipelineInput

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/26 10:41:30

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System.Collections.Generic;
using CenturyGame.Core.Pipeline;

namespace CenturyGame.Core.Pipeline
{
    public abstract class BasePipelineInput : IPipelineInput
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        private readonly Dictionary<string, System.Object> _dataDic = new Dictionary<string, object>();

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        public abstract string Desc { set; get; }

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        public virtual T GetData<T>(string key, T defaultVal)
        {
            System.Object obj;

            if (this._dataDic.TryGetValue(key, out obj))
            {
                return (T)obj;
            }

            return defaultVal;
        }

        public virtual void SetData(string key, object arg)
        {
            if (!this._dataDic.ContainsKey(key))
            {
                this._dataDic.Add(key, arg);
            }
            else
            {
                this._dataDic[key] = arg;
            }
        }

        #endregion

    }
}
