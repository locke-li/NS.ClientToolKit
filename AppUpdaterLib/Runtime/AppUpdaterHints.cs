/**************************************************************
 *  类名称：          AppUpdaterHints
 *  描述：
 *  作者：            Chico(wuyuanbing)
 *  创建时间：        2020/12/21 11:30:41
 *  最后修改人：
 *  版权所有 （C）:   CenturyGames
 **************************************************************/

using System;

namespace CenturyGame.AppUpdaterLib.Runtime
{
    internal class AppUpdaterHints
    {
        private static AppUpdaterHints smInstance;
        public static AppUpdaterHints Instance
        {
            get
            {
                if (smInstance == null)
                {
                    smInstance = new AppUpdaterHints();
                }
                return smInstance;
            }
        }

        /// <summary>
        /// 保存lua到本地路径名小写（在可写空间后）
        /// </summary>
        public bool LowerLuaName = false;

        /// <summary>
        /// 手动执行app更新
        /// </summary>
        public bool ManualPerformAppUpdate = false;

        /// <summary>
        /// 激活表数据更新
        /// </summary>
        public bool EnableTableDataUpdate = true;

        public void SetHintValue(AppUpdaterHintName hintName , int hintVal)
        {
            switch (hintName)
            {
                case AppUpdaterHintName.LOWER_LUA_NAME:
                    if (hintVal == (int)AppUpdaterBool.FALSE)
                    {
                        LowerLuaName = false;
                    }
                    else if (hintVal == (int)AppUpdaterBool.TRUE)
                    {
                        LowerLuaName = true;
                    }
                    else
                    {
                        throw new ArgumentException($"hintName : {hintName}  , hintVal : {hintVal} .");
                    }
                    break;
                case AppUpdaterHintName.MANUAL_PERFORM_APP_UPDATE:
                    if (hintVal == (int)AppUpdaterBool.FALSE)
                    {
                        ManualPerformAppUpdate = false;
                    }
                    else if (hintVal == (int)AppUpdaterBool.TRUE)
                    {
                        ManualPerformAppUpdate = true;
                    }
                    else
                    {
                        throw new ArgumentException($"hintName : {hintName}  , hintVal : {hintVal} .");
                    }
                    break;
                case AppUpdaterHintName.ENABLE_TABLE_DATA_UPDATE:
                    if (hintVal == (int)AppUpdaterBool.FALSE)
                    {
                        EnableTableDataUpdate = false;
                    }
                    else if (hintVal == (int)AppUpdaterBool.TRUE)
                    {
                        EnableTableDataUpdate = true;
                    }
                    else
                    {
                        throw new ArgumentException($"hintName : {hintName}  , hintVal : {hintVal} .");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}