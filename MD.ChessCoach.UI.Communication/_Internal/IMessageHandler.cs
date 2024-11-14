using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal;

internal interface IMessageHandler
{
    /// <summary>
    /// Sends a request to the server and returns the UUID.
    /// </summary>
    /// <param name="action">The requested action.</param>
    /// <param name="args">The arguments for the payload.</param>
    /// <returns>The UUID of the sent message.</returns>
    string SendRequest(string action, Dictionary<string, string> args);
    
    /// <summary>
    /// Sends a response to the server and returns the UUID.
    /// </summary>
    /// <returns>The UUID of the sent message.</returns>
    string SendResponse(int responseCode, string responseMessage);
}