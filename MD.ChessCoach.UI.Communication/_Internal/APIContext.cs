using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using MD.ChessCoach.UI.Communication._Internal.Connectivity;
using MD.ChessCoach.UI.Communication._Internal.Endpoints;
using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal;

/// <summary>
/// Provides context for the API.
/// </summary>
internal class APIContext
{
    private Connector _connector;
    private Socket? _socket;

    private RegistrationHandler _registrationHandler;
    private Logger _logHandler;
    
    public Logger Logger => _logHandler;
    
    public APIContext()
    {
        _connector = new Connector(ProcessReceivedMessage);
        _socket = _connector.ConnectToRemote(Constants.ServerHost, Constants.ServerPort, Constants.ComponentPort);

        if (_socket == null)
            throw new Exception("Failed to connect to server");

        IMessageHandler messageHandler = new MessageHandler(_connector, _socket);
            
        _registrationHandler = new RegistrationHandler(_connector, _socket);
        _logHandler = new Logger(messageHandler);
            
        _registrationHandler.Register();
    }

    [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
    public void Shutdown()
    {
        if (_socket == null)
            throw new InvalidOperationException("Socket is not initialized");
        
        if (!Constants.KeepServerAlive)
#pragma warning disable CS0162 // Unreachable code detected
        {
            SendShutdownMessage();
        }
#pragma warning restore CS0162 // Unreachable code detected
        
        _registrationHandler?.Unregister();
        
        _connector.Shutdown();
        _socket?.Close();
    }
    
    private void SendShutdownMessage()
    {
        if (_socket == null)
            throw new InvalidOperationException("Socket is not initialized");
        
        MessagePayload payload = PayloadFactory.BuildPayload(
            action: "shutdown",
            args: []);
        
        Message message = new Message(payload);
            
        _connector.SendMessage(_socket, message);
    }


    private void ProcessReceivedMessage(MessagePayload payload)
    {
        Console.WriteLine($"Gotten: {payload.action}");
    }
}