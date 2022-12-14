using Niantic.ARDK.Utilities.Logging;
using UnityEngine;

namespace Niantic.ARVoyage
{
    public class UnityLogStackTraceEnabler
    {
        /// <summary>
        /// Overrides ARDK 1.3 behavior that disables log stack traces by default
        /// when the UnityARLogHandler instance is created
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnableUnityLogStackTraces()
        {
            UnityARLogHandler ardkLogHandlerInstance = UnityARLogHandler.Instance;
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
        }
    }
}
