//#define SOCKETIO_LOGGER_DEBUG_LOGS
#define SOCKETIO_LOGGER_DEBUG_WARNINGS
#define SOCKETIO_LOGGER_DEBUG_ERRORS
#define SOCKETIO_LOGGER_DEBUG_EXCEPTIONS

namespace SocketIO
{
    using UnityEngine;

    public static class Logger
    {
        public static void Log(object obj)
        {
#if SOCKETIO_LOGGER_DEBUG_LOGS
            Debug.Log(obj);
#endif
        }

        public static void LogWarning(object obj)
        {
#if SOCKETIO_LOGGER_DEBUG_WARNINGS
            Debug.LogWarning(obj);
#endif
        }

        public static void LogError(object obj)
        {
#if SOCKETIO_LOGGER_DEBUG_ERRORS
            Debug.LogError(obj);
#endif
        }

        public static void LogException(string context, System.Exception e)
        {
#if SOCKETIO_LOGGER_DEBUG_EXCEPTIONS
            LogError(string.Format("{0} Received Error. Message: {1} Stack Trace: {2}", context, e.Message, e.StackTrace));
#endif
        }

    }
}
