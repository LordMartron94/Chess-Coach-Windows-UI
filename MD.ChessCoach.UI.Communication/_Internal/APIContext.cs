using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using MD.ChessCoach.UI.Communication._Internal.Connectivity;
using MD.ChessCoach.UI.Communication._Internal.Endpoints;
using MD.ChessCoach.UI.Communication._Internal.Processing;
using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal;

/// <summary>
/// Provides context for the API.
/// </summary>
internal class APIContext
{
    private readonly Connector _connector;
    private readonly Socket? _socket;

    private readonly RegistrationHandler _registrationHandler;

    public Logger Logger { get; }
    public ChessEndpoint ChessEndpoint { get; }
    

    public APIContext()
    {
        MessageProcessorSelector messageProcessor = new MessageProcessorSelector();
        _connector = new Connector(messageProcessor);
        _socket = _connector.ConnectToRemote(Constants.ServerHost, Constants.ServerPort, Constants.ComponentPort);

        if (_socket == null)
            throw new Exception("Failed to connect to server");

        IMessageHandler messageHandler = new MessageHandler(_connector, _socket);
        messageProcessor.Initialize(messageHandler);
            
        _registrationHandler = new RegistrationHandler(_connector, _socket);
        
        Logger = new Logger(messageHandler);
        ChessEndpoint = new ChessEndpoint(messageHandler);
            
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
}