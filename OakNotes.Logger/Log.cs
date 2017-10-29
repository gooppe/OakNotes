using NLog;

namespace OakNotes.Logger
{
    public static class Log
    {
        public static readonly NLog.Logger Intance = LogManager.GetCurrentClassLogger();
    }
}
