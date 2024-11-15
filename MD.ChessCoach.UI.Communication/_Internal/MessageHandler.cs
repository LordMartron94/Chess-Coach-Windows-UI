using System.Net.Sockets;
using MD.ChessCoach.UI.Communication._Internal.Connectivity;
using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal;

internal class MessageHandler : IMessageHandler
{
    private readonly Connector _connector;
    private readonly Socket _socket;

    internal MessageHandler(Connector connector, Socket socket)
    {
        _connector = connector;
        _socket = socket;
    }

    public string SendRequest(string action, Dictionary<string, string> args)
    {
        MessagePayload payload = PayloadFactory.BuildPayload(action, args);
        Message message = new Message(payload);
        
        _connector.SendMessage(_socket, message);
        
        return message.unique_id;
    }

    public string SendResponse(int responseCode, string responseMessage)
    {
        MessagePayload payload = PayloadFactory.BuildPayload(
            action: "response",
            args: 
            [
                ("string", responseMessage), 
                ("int", responseCode.ToString())
            ]);
        
        Message message = new Message(payload);
        _connector.SendMessage(_socket, message);
        
        return message.unique_id;
    }

}