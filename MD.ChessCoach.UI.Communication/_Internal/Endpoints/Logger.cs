namespace MD.ChessCoach.UI.Communication._Internal.Endpoints;

/// <summary>
/// Logging endpoint for logging messages.
/// </summary>
internal class Logger : AbEndpoint
{
    private readonly Dictionary<LogLevel, string> _logLevels = new Dictionary<LogLevel, string>();
    
    public Logger(IMessageHandler messageHandler) : base(messageHandler)
    {
        _logLevels[LogLevel.Debug] = "debug";
        _logLevels[LogLevel.Info] = "info";
        _logLevels[LogLevel.Warning] = "warning";
        _logLevels[LogLevel.Error] = "error";
        _logLevels[LogLevel.Critical] = "critical";
    }
    
    /// <summary>
    /// Logs a message to the server using the specified log level and optional module separator.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="level">The log level of the message. Default is LogLevel.Info.</param>
    /// <param name="moduleSeparator">An optional separator to be added between the module name and the message. Default is an empty string.</param>
    /// <param name="forceShow">A flag indicating whether the message should be shown regardless of the log level settings. Default is false.</param>
    public void Log(string message, LogLevel level = LogLevel.Info, string moduleSeparator = "", bool forceShow = false)
    {
        MessageHandler.SendRequest(
            action: $"log_{_logLevels[level]}", 
            args:
            [
                ("string", message),
                ("bool", forceShow.ToString()),
                ("string", ""),
                ("string", moduleSeparator)
            ],
            onResponseReceived: null);
    }
}