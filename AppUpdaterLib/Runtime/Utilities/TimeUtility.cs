/***************************************************************

 *  类名称：        Time

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/13 17:20:33

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;

namespace CenturyGame.AppUpdaterLib.Runtime.Utilities
{
    public static class TimeUtility
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        public static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

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

        /// <summary>
        /// 获取但其那
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentTimeSeconds()
        {
            var now = DateTime.Now;

            var diff = now - EpochTime;

            long totalSeconds = 0;

            unchecked
            {
                totalSeconds = (long)diff.TotalSeconds;
            }

            return (long)totalSeconds;
        }

        #endregion

    }
}
