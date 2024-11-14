using MD.ChessCoach.UI.Communication._Internal;

namespace MD.ChessCoach.UI.Communication;

/// <summary>
///     API interface for communication with the server.
/// </summary>
/// <remarks>
/// This class is a facade for the communication logic.
/// </remarks>
public sealed class API
{
    private static volatile API? _instance;
    private readonly static object SyncRoot = new object();
    
    private readonly APIContext _context;
    
    private API()
    {
        _context = new APIContext();
    }

    public static API Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            lock (SyncRoot)
            {
                // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                if (_instance == null)
                    _instance = new API();
            }

            return _instance;
        }
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
        _context.Logger.Log(message, level, moduleSeparator, forceShow);
    }
    
    
    public void Shutdown()
    {
        _context.Shutdown();
    }
}