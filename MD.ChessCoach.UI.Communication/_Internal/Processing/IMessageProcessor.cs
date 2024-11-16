using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal.Processing;

/// <summary>
/// Interface for processing incoming messages.
/// </summary>
public interface IMessageProcessor
{
    /// <summary>
    /// Processes the given message.
    /// </summary>
    /// <param name="incomingMessage">The message to process.</param>
    void ProcessIncomingMessage(Message incomingMessage);
}