/***************************************************************

 *  类名称：        LuaAccessManager

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/1 11:27:26

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.LuaModule.Runtime.Interfaces;
using System;
using System.Runtime.CompilerServices;

namespace CenturyGame.LuaModule.Runtime
{
    public static class LuaAccessManager
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        private static bool mInitialized = false;

        private static ILuaEnvironment s_mEnv = null;

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        public static ILuaEnvironment Environment => s_mEnv;

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        public static void Init(ILuaEnvironment env)
        {
            if (mInitialized)
                return;
            s_mEnv = env ?? throw new ArgumentNullException(nameof(env));
            mInitialized = true;
        }

        /// <summary>
        /// 创建一个table
        /// </summary>
        /// <returns></returns>
        public static ILuaTable NewLuaTable()
        {
            CheckEnv(nameof(NewLuaTable));
            return s_mEnv.NewTable();
        }

        /// <summary>
        /// 获取当前全局Table
        /// </summary>
        /// <returns></returns>
        public static ILuaTable GetGlobal()
        {
            CheckEnv(nameof(GetGlobal));
            return s_mEnv.Global;
        }


        public static ILuaTable GetGlobalTable(string name)
        {
            CheckEnv($"func name : {nameof(GetGlobalTable)} argument name : {name} .");
            var global = s_mEnv.Global;

            ILuaTable table;
            global.GetTable(name, out table);
            return table;
        }

        public static ILuaFunction GetGlobalFunction(string name)
        {
            CheckEnv($"func name : {nameof(GetGlobalFunction)} argument name : {name} .");
            var global = s_mEnv.Global;

            ILuaFunction func;
            global.GetFunction(name,out func);
            return func;
        }

        private static void CheckEnv(string methodName)
        {
            if(s_mEnv == null)
                throw new NullReferenceException($"The current lua environment is null ,current function name is \"{methodName}\" .");
        }

        #endregion

    }
}
