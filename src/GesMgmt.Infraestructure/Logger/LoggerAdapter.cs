using NLog;

namespace GesMgmt.Infraestructure.Logger
{
    public class LoggerAdapter : IAppLogger
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        public void LogInfo(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        public void LogWarn(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

        public void LogError(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.Error(exception, message, args);
        }
    }
}
