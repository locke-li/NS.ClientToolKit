/***************************************************************

 *  类名称：        Const

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/3 11:18:58

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


namespace CenturyGame.Log4NetForUnity.Runtime
{
    public class Const
    {
        public const string ConfigName = "log4net";
        public const string ConfigSuffix = ".xml";

        public const string DefaultConfig =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
            "<log4net>\r\n" +
            "	<appender name=\"unityConsole\" type=\"CenturyGame.Log4NetForUnity.Runtime.UnityDefaultLogAppender\">\r\n" +
            "		<layout type=\"log4net.Layout.PatternLayout\">\r\n" +
            "			<conversionPattern value=\"[%level][%logger] %message\"/>\r\n" +
            "		</layout>\r\n" +
            "	</appender>\r\n" +
            "	<root>\r\n" +
            "		<level value=\"DEBUG\"/>\r\n" +
            "		<appender-ref ref=\"unityConsole\"/>\r\n" +
            "	</root>\r\n" +
            "</log4net>";

    }
}
