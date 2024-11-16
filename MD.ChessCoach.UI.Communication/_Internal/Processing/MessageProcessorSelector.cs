using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal.Processing;

internal class MessageProcessorSelector : IMessageProcessor
{
    private IMessageProcessor? _responseProcessor;
    private IMessageProcessor? _requestProcessor;

    private bool _initialized;

    public void Initialize(IMessageHandler messageHandler)
    {
        _responseProcessor = new ResponseProcessor(messageHandler);
        _requestProcessor = new RequestProcessor(messageHandler);
        _initialized = true;
    }
    
    public void ProcessIncomingMessage(Message incomingMessage)
    {
        if (!_initialized)
            throw new InvalidOperationException("Message processor selector is not initialized");
        
        switch (incomingMessage.payload.action)
        {
            case "response" or "error":
                _responseProcessor!.ProcessIncomingMessage(incomingMessage);
                break;
            default:
                _requestProcessor!.ProcessIncomingMessage(incomingMessage);
                break;
        }
    }
}