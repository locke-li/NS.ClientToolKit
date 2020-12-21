/**************************************************************
 *  类名称：          AppUpdaterHints
 *  描述：
 *  作者：            Chico(wuyuanbing)
 *  创建时间：        2020/12/21 11:30:41
 *  最后修改人：
 *  版权所有 （C）:   CenturyGames
 **************************************************************/

using System;
using CenturyGame.AppUpdaterLib.Runtime;

namespace CenturyGame.ClientToolKit.AppUpdaterLib.Runtime
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
        

        public void SetHintValue(AppUpdaterHintName hintName , int hitVal)
        {
            switch (hintName)
            {
                case AppUpdaterHintName.LOWER_LUA_NAME:
                    if (hitVal == (int)AppUpdaterBool.FALSE)
                    {
                        LowerLuaName = false;
                    }
                    else if (hitVal == (int)AppUpdaterBool.TRUE)
                    {
                        LowerLuaName = true;
                    }
                    else
                    {
                        throw new ArgumentException($"hintName : {hintName}  , hitVal : {hitVal} .");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}