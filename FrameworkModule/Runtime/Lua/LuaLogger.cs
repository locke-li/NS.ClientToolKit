using System;
using CenturyGame.LoggerModule.Runtime;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;

namespace CenturyGame.Framework.Lua
{
    public static class LuaLogger
    {
        private static readonly Lazy<ILogger> s_mLogger = new Lazy<ILogger>(() =>
            LoggerManager.GetLogger("LUA"));

        public static void LogDebug(object message)
        {
            s_mLogger.Value.Debug(message);
        }

        public static void LogWarn(object message)
        {
            s_mLogger.Value.Warn(message);
        }

        public static void LogError(object message)
        {
            s_mLogger.Value.Error(message);
        }
    }
}