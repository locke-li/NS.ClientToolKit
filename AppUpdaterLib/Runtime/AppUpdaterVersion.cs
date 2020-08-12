/***************************************************************

 *  类名称：        AppUpdaterVersion

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/7 10:21:20

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CenturyGame.AppUpdaterLib.Runtime
{
    public class AppUpdaterVersion
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        /// <summary>
        /// 当前程序集版本
        /// </summary>
        public const string AssemblyVersion = "0.0.1";

        /// <summary>
        /// 版本后缀信息
        /// </summary>
        public const string VersionSuffix = "PreAlpha";

        public static readonly string EngineVersion = "";

        #endregion

        //--------------------------------------------------------------

        #region Properties & Events

        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------

        #region Creation & Cleanup

        //--------------------------------------------------------------

        static AppUpdaterVersion()
        {
            EngineVersion = Application.unityVersion;
        }

        #endregion

        //--------------------------------------------------------------

        #region Methods

        //--------------------------------------------------------------


        public static string GetVersionInfo()
        {
            return $"{AssemblyVersion}-{VersionSuffix}-{EngineVersion}";
        }

        #endregion

    }
}
