using System.Net.Sockets;
using MD.ChessCoach.UI.Communication._Internal.Connectivity;
using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal;

/// <summary>
/// Handles the logic of registering the component to the server.
/// </summary>
internal class RegistrationHandler
{
    private readonly Connector _connector;
    private readonly Socket _socket;
    
    public RegistrationHandler(Connector connector, Socket socket)
    {
        _connector = connector;
        _socket = socket;
    }
    
    public void Register()
    {
        Message message = Utilities.GetMessageFromSharedResource(Constants.RegistrationEmbeddedResourcePath);
        _connector.SendMessage(_socket, message);
        _connector.StartKeepAliveMessageLoop(_socket);
    }

    public void Unregister()
    {
        MessagePayload payload = PayloadFactory.BuildPayload(
            action: "unregister",
            args: new Dictionary<string, string>());
        
        Message message = new Message(payload);
        
        _connector.SendMessage(_socket, message);
    }
}