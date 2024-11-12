using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using MD.ChessCoach.UI.Communication._Internal;
using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication;

/// <summary>
///     API interface for communication with the server.
/// </summary>
public sealed class API
{
    private static volatile API? _instance;
    private readonly static object SyncRoot = new object();

    private Connector? _connector;

    private readonly Dictionary<LogLevel, string> _logLevels = new Dictionary<LogLevel, string>();

    private Socket? _socket;
    private API()
    {
        Initialize();
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

    private void Initialize()
    {
        _logLevels[LogLevel.Debug] = "debug";
        _logLevels[LogLevel.Info] = "info";
        _logLevels[LogLevel.Warning] = "warning";
        _logLevels[LogLevel.Error] = "error";
        _logLevels[LogLevel.Critical] = "critical";

        try
        {
            _connector = new Connector(ProcessReceivedMessage);
            _socket = _connector.ConnectToRemote(Constants.ServerHost, Constants.ServerPort, Constants.ComponentPort);

            if (_socket == null)
                throw new Exception("Failed to connect to server");
        }
        catch (SocketException ex)
        {
            // Just in case
            _socket?.Close();
            Console.WriteLine(ex.ErrorCode == 10048 ? "WARNING: Port is already in use." : ex.Message);
            return;
        }
        catch (Exception ex)
        {
            // Just in case
            _socket?.Close();
            throw new Exception("Failed to initialize API: " + ex.Message, ex);
        }

        Message message = Utilities.GetMessageFromSharedResource(Constants.RegistrationEmbeddedResourcePath);
        _connector?.SendMessage(_socket, message);
        _connector?.StartKeepAliveMessageLoop(_socket);
    }

    [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
    public void Shutdown()
    {
        if (_socket == null)
            throw new InvalidOperationException("Socket is not initialized");
        
        if (!Constants.IsDebugMode)
#pragma warning disable CS0162 // Unreachable code detected
        {
            Message shutdownMessage = Utilities.GetMessageFromSharedResource(Constants.DefaultRequestResourcePath, Utilities.BuildPayload("shutdown"));
            _connector?.SendMessage(_socket, shutdownMessage);
            
            Message unregisterMessage = Utilities.GetMessageFromSharedResource(Constants.DefaultRequestResourcePath, Utilities.BuildPayload("unregister"));
            _connector?.SendMessage(_socket, unregisterMessage);
        }
#pragma warning restore CS0162 // Unreachable code detected
        
        _connector?.Shutdown();
        _socket?.Close();
    }

    public void Log(string message, LogLevel level = LogLevel.Info, string moduleSeparator = "", bool forceShow = false)
    {
        if (_socket == null)
            throw new InvalidOperationException("Socket is not initialized");
        
        MessagePayload payload = new MessagePayload
        {
            action = $"log_{_logLevels[level]}",
            args =
            [
                new Argument { type = "string", value = message },
                new Argument { type = "bool", value = forceShow.ToString() },
                new Argument { type = "string", value = "" },
                new Argument { type = "string", value = moduleSeparator }
            ]
        };

        Message transformedMessage = Utilities.GetMessageFromSharedResource(Constants.DefaultRequestResourcePath, payload);
        _connector?.SendMessage(_socket, transformedMessage);
    }

    private void ProcessReceivedMessage(MessagePayload payload)
    {
        Console.WriteLine($"Gotten: {payload.action}");
    }
}