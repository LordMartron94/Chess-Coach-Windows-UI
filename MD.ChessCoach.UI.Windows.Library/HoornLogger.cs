using MD.ChessCoach.UI.Communication;
using MD.Common.Utils;

namespace MD.ChessCoach.UI.Windows.Library;

/// <summary>
///     Class for logging using the Hoorn library.
///     Uses the API for centralized logging across components.
/// </summary>
public sealed class HoornLogger : ILogger
{
    private const string DefaultModuleSeparator = Constants.RootModuleSeparator + ".Debugging";

    private static volatile HoornLogger? _instance;
    private readonly static object SyncRoot = new object();
    private API? _api;

    private HoornLogger()
    {

    }

    public static HoornLogger Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            lock (SyncRoot)
            {
                // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                if (_instance == null)
                    _instance = new HoornLogger();
            }

            return _instance;
        }
    }

    /// <summary>
    ///     Logs a message with a debug level.
    /// </summary>
    /// <remarks>
    ///     Use this for very granular information helpful in diagnosing intricate issues during development or
    ///     troubleshooting.
    ///     <para>
    ///         <b>Objective Conditions:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>High volume of messages expected.</item>
    ///         <item>Information that would only be useful to a developer, not an end-user or system administrator.</item>
    ///         <item>Data that might change frequently (e.g., variable values, function entry/exit).</item>
    ///     </list>
    ///     <para>
    ///         <b>Examples:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>"Entered function calculate_tax with arguments: income=50000, deductions=10000"</item>
    ///         <item>"Loop iteration 5: current value = 12"</item>
    ///     </list>
    /// </remarks>
    /// <param name="message">The message to log.</param>
    /// <param name="forceShow">Whether to show the error regardless of logger display settings.</param>
    /// <param name="moduleSeparator">The module separator RELATIVE to the root.</param>
    public void Debug(string message, bool? forceShow = null, string? moduleSeparator = null)
    {
        Log(message, LogLevel.Debug, moduleSeparator ?? DefaultModuleSeparator, forceShow ?? false);
    }

    /// <summary>
    ///     Logs a message with an info level.
    /// </summary>
    /// <remarks>
    ///     Use this to record normal application events and milestones. Think of it as a high-level overview of what the
    ///     application is doing.
    ///     <para>
    ///         <b>Objective Conditions:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Relatively frequent messages, but less verbose than debug.</item>
    ///         <item>Information that tracks the general flow of the application.</item>
    ///         <item>Data that confirms key processes are executing as expected.</item>
    ///     </list>
    ///     <para>
    ///         <b>Examples:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>"User 'john_doe' logged in successfully."</item>
    ///         <item>"Order #12345 processed and dispatched."</item>
    ///         <item>"Database connection established."</item>
    ///     </list>
    /// </remarks>
    /// <param name="message">The message to log.</param>
    /// <param name="forceShow">Whether to show the error regardless of logger display settings.</param>
    /// <param name="moduleSeparator">The module separator RELATIVE to the root.</param>
    public void Info(string message, bool? forceShow = null, string? moduleSeparator = null)
    {
        Log(message, LogLevel.Info, moduleSeparator ?? DefaultModuleSeparator, forceShow ?? false);
    }

    /// <summary>
    ///     Logs a warning.
    /// </summary>
    /// <remarks>
    ///     Use this to highlight potential problems or unexpected situations that don't necessarily prevent the application
    ///     from continuing, but might need attention.
    ///     <para>
    ///         <b>Objective Conditions:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Infrequent occurrences.</item>
    ///         <item>Situations that could lead to errors if not addressed.</item>
    ///         <item>Deviations from expected behavior that might require investigation.</item>
    ///     </list>
    ///     <para>
    ///         <b>Examples:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>"Low disk space on server."</item>
    ///         <item>"API request timed out, retrying..."</item>
    ///         <item>"User attempted to access unauthorized resource."</item>
    ///     </list>
    /// </remarks>
    /// <param name="message">The message to log.</param>
    /// <param name="forceShow">Whether to show the error regardless of logger display settings.</param>
    /// <param name="moduleSeparator">The module separator RELATIVE to the root.</param>
    public void Warning(string message, bool? forceShow = null, string? moduleSeparator = null)
    {
        Log(message, LogLevel.Warning, moduleSeparator ?? DefaultModuleSeparator, forceShow ?? false);
    }

    /// <summary>
    ///     Logs an error.
    /// </summary>
    /// <remarks>
    ///     Use this to indicate errors that prevent the current operation from completing successfully, but don't crash the
    ///     entire application.
    ///     <para>
    ///         <b>Objective Conditions:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Events that disrupt the normal flow of the application.</item>
    ///         <item>Situations requiring user intervention or recovery mechanisms.</item>
    ///         <item>Exceptions caught and handled within the application.</item>
    ///     </list>
    ///     <para>
    ///         <b>Examples:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>"Database query failed: [SQL error message]"</item>
    ///         <item>"Could not connect to external service."</item>
    ///         <item>"Invalid input provided by the user."</item>
    ///     </list>
    /// </remarks>
    /// <param name="message">The message to log.</param>
    /// <param name="forceShow">Whether to show the error regardless of logger display settings.</param>
    /// <param name="moduleSeparator">The module separator RELATIVE to the root.</param>
    public void Error(string message, bool? forceShow = null, string? moduleSeparator = null)
    {
        Log(message, LogLevel.Error, moduleSeparator ?? DefaultModuleSeparator, forceShow ?? false);
    }

    /// <summary>
    ///     Logs a critical error.
    /// </summary>
    /// <remarks>
    ///     Use this for severe errors that threaten the stability or integrity of the entire application.
    ///     <para>
    ///         <b>Objective Conditions:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Very rare occurrences.</item>
    ///         <item>Events that might lead to application crashes or data loss.</item>
    ///         <item>Situations requiring immediate attention from system administrators.</item>
    ///     </list>
    ///     <para>
    ///         <b>Examples:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>"Unhandled exception: [exception details]"</item>
    ///         <item>"System memory exhausted."</item>
    ///         <item>"Critical security breach detected."</item>
    ///     </list>
    /// </remarks>
    /// <param name="message">The message to log.</param>
    /// <param name="forceShow">Whether to show the error regardless of logger display settings.</param>
    /// <param name="moduleSeparator">The module separator RELATIVE to the root.</param>
    public void Critical(string message, bool? forceShow = null, string? moduleSeparator = null)
    {
        Log(message, LogLevel.Critical, moduleSeparator ?? DefaultModuleSeparator, forceShow ?? false);
    }

    public void Initialize(API api)
    {
        _api = api;
    }

    private void Log(string message, LogLevel level, string moduleSeparator, bool forceShow)
    {
        if (_api == null)
        {
            Console.WriteLine($"API not initialized. Log message: {message}");
            return;
        }

        string handledSeparator;

        if (moduleSeparator != DefaultModuleSeparator)
            handledSeparator = Constants.RootModuleSeparator + "." + moduleSeparator;
        else handledSeparator = moduleSeparator;

        _api.Log(message, level, handledSeparator, forceShow);
    }
}