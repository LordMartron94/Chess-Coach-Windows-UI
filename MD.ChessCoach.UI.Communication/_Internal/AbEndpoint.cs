namespace MD.ChessCoach.UI.Communication._Internal;

/// <summary>
/// Abstract base class for all endpoints.
/// </summary>
internal abstract class AbEndpoint
{
    protected readonly IMessageHandler MessageHandler;
    
    protected AbEndpoint(IMessageHandler messageHandler)
    {
        MessageHandler = messageHandler;
    }
}