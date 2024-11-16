using System.Net.Sockets;
using MD.ChessCoach.UI.Communication._Internal.Connectivity;
using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal;

internal class MessageHandler : IMessageHandler
{
    private readonly Connector _connector;
    private readonly Socket _socket;
    
    private readonly List<Action<Message, Action<Message>?>> _sendMessageActions;
    

    internal MessageHandler(Connector connector, Socket socket)
    {
        _connector = connector;
        _socket = socket;
        _sendMessageActions = new List<Action<Message, Action<Message>?>>();
    }

    public void SubscribeToSendMessage(Action<Message, Action<Message>?> sendMessageAction)
    {
        if (_sendMessageActions.Contains(sendMessageAction))
            return;
        
        _sendMessageActions.Add(sendMessageAction);
    }

    public void UnsubscribeFromSendMessage(Action<Message, Action<Message>?> sendMessageAction)
    {
        if (!_sendMessageActions.Contains(sendMessageAction))
            return;
        
        _sendMessageActions.Remove(sendMessageAction);
    }

    public string SendRequest(string action, IEnumerable<(string, string)> args, Action<Message>? onResponseReceived = null)
    {
        MessagePayload payload = PayloadFactory.BuildPayload(action, args);
        Message message = new Message(payload);
        
        _connector.SendMessage(_socket, message);
        
        InformSubscriptions(message, onResponseReceived);
        
        return message.unique_id;
    }

    public string SendResponse(int responseCode, string responseMessage, string targetID)
    {
        MessagePayload payload = PayloadFactory.BuildPayload(
            action: "response",
            args: 
            [
                ("string", responseMessage), 
                ("int", responseCode.ToString())
            ]);
        
        Message message = new Message(payload, targetID);
        _connector.SendMessage(_socket, message);
        
        return message.unique_id;
    }

    private void InformSubscriptions(Message message, Action<Message>? onResponseReceived = null)
    {
        foreach (Action<Message, Action<Message>?> sendMessageAction in _sendMessageActions)
            sendMessageAction(message, onResponseReceived);
    }
}