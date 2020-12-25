/**************************************************************
 *  类名称：          AppSetting
 *  描述：
 *  作者：            Chico(wuyuanbing)
 *  创建时间：        2020/10/19 11:15:40
 *  最后修改人：
 *  版权所有 （C）:   CenturyGames
 **************************************************************/

using UnityEngine;

namespace CenturyGame.AppSetting.Runtime
{

    public class AppSetting : ScriptableObject
    {
        private static AppSetting mInstance = null;
        public static AppSetting Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = Resources.Load<AppSetting>("AppSetting");
                return mInstance;
            }
        }
    }
}