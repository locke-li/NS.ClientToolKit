/***************************************************************

 *  类名称：        LuaPluginCreateFactory

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/1 20:53:36

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.LuaModule.Runtime.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CenturyGame.LuaModule.Runtime
{
    public static class LuaPluginCreateFactory
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

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


        public static ILuaPlugin Create(string typeFullName)
        {
            Type targetType = null;
            AppDomain domain = AppDomain.CurrentDomain;
            Assembly[] assemblies = domain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.GetName().Name == "Assembly-CSharp")
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.FullName == typeFullName)
                        {
                            targetType = type;
                            break;
                        }
                    }
                    break;
                }
            }

            if (targetType == null)
            {
                throw new TypeLoadException(typeFullName);
            }
            var luaPlugin = Activator.CreateInstance(targetType) as ILuaPlugin;
            luaPlugin.OnInit();

            return luaPlugin;
        }

        #endregion

    }
}
