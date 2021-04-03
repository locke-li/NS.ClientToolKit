/***************************************************************

 *  类名称：        AppUpdaterConfigManager

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2021/2/24 16:32:50

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenturyGame.AppUpdaterLib.Runtime.Configs;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace CenturyGame.AppUpdaterLib.Runtime.Managers
{
    public class AppUpdaterConfigManager
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        private static AppUpdaterConfig mAppUpdaterConfig;

        public static AppUpdaterConfig AppUpdaterConfig
        {
            get
            {
                if (mAppUpdaterConfig == null)
                {
                    var appUpdaterConfigText = Resources.Load<TextAsset>("appupdater");
                    mAppUpdaterConfig = JsonUtility.FromJson<AppUpdaterConfig>(appUpdaterConfigText.text);
                }

                return mAppUpdaterConfig;
            }
            set => mAppUpdaterConfig = value;
        }
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
