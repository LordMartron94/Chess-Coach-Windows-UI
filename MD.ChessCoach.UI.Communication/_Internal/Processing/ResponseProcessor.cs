using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal.Processing;

internal class ResponseProcessor : IMessageProcessor
{
    private readonly Dictionary<Message, Action<Message>?> _sentMessagesExpectingAResponseWithSubscriber;
    
    public ResponseProcessor(IMessageHandler messageHandler)
    {
        _sentMessagesExpectingAResponseWithSubscriber = new Dictionary<Message, Action<Message>?>();
        
        messageHandler.SubscribeToSendMessage(AddSentMessage);
    }

    private void AddSentMessage(Message message, Action<Message>? subscriber)
    {
        _sentMessagesExpectingAResponseWithSubscriber.TryAdd(message, subscriber);
    }
    
    public void ProcessIncomingMessage(Message incomingMessage)
    {
        Message? associatedSentMessage = FindSentMessageForIncomingResponse(incomingMessage);
        
        if (associatedSentMessage == null)
            return;

        bool isServerResponse = incomingMessage.payload.args.Last().value == "false";
        
        if (isServerResponse)
            HandleServerResponse(incomingMessage, associatedSentMessage.Value);
        else HandleClientResponse(incomingMessage, associatedSentMessage.Value);
    }

    private Message? FindSentMessageForIncomingResponse(Message incomingResponse)
    {
        foreach (KeyValuePair<Message, Action<Message>?> sentMessage in _sentMessagesExpectingAResponseWithSubscriber.Where(sentMessage => sentMessage.Key.unique_id == incomingResponse.target_id))
            return sentMessage.Key;
        
        return null;
    }

    private void HandleServerResponse(Message message, Message associatedSentMessage)
    {
        if (message.payload.action == "error")
        {
            HandleClientResponse(message, associatedSentMessage); // Want to treat error as if it was client response, otherwise subscription is not aware of error.
            return;
        }
        
        int argLength = message.payload.args.Length;
        bool responseExpected = argLength > 2 && message.payload.args[argLength - 2].value == "true";
        
        if (!responseExpected)
            _sentMessagesExpectingAResponseWithSubscriber.Remove(associatedSentMessage);
    }
    
    private void HandleClientResponse(Message incomingMessage, Message associatedSentMessage)
    {
        Action<Message>? onClientResponseReceived = _sentMessagesExpectingAResponseWithSubscriber[associatedSentMessage];
        onClientResponseReceived?.Invoke(incomingMessage);
        _sentMessagesExpectingAResponseWithSubscriber.Remove(associatedSentMessage);
    }
}