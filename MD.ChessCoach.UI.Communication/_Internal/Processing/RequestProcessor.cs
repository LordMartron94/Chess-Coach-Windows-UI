using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal.Processing;

internal class RequestProcessor : IMessageProcessor
{
    private readonly IMessageHandler _messageHandler;
    
    public RequestProcessor(IMessageHandler messageHandler)
    {
        _messageHandler = messageHandler;
    }
    
    public void ProcessIncomingMessage(Message incomingMessage)
    {
        // This will obviously never happen in a real-world scenario (because the server routes only back what we ourselves have registered), but it's a placeholder for demonstration purposes.
        _messageHandler.SendResponse(355, "Unknown request. Can't process this one.", incomingMessage.unique_id);
    }
}