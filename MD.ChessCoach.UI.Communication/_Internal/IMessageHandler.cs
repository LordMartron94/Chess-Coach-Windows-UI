using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal;

internal interface IMessageHandler
{
    /// <summary>
    /// Subscribes to the send message event.
    /// </summary>
    /// <param name="sendMessageAction">The action to be executed when a message is sent. It takes a Message object as a parameter.</param>
    void SubscribeToSendMessage(Action<Message, Action<Message>?> sendMessageAction);
    
    /// <summary>
    /// Unsubscribes from the send message event.
    /// </summary>
    /// <param name="sendMessageAction">The action to be removed from the subscription. It should match the action that was previously subscribed.</param>
    void UnsubscribeFromSendMessage(Action<Message, Action<Message>?> sendMessageAction);

    /// <summary>
    /// Sends a request to the server and returns the UUID.
    /// </summary>
    /// <param name="action">The requested action.</param>
    /// <param name="args">The arguments for the payload.</param>
    /// <param name="onResponseReceived">A callback for when a response is received for the request.</param>
    /// <returns>The UUID of the sent message.</returns>
    /// <remarks>
    /// The response received callback is invoked when either a client response or a server error response is received.
    /// </remarks>
    string SendRequest(string action, IEnumerable<(string, string)> args, Action<Message>? onResponseReceived = null);
    
    /// <summary>
    /// Sends a response to the server and returns the UUID.
    /// </summary>
    /// <returns>The UUID of the sent message.</returns>
    string SendResponse(int responseCode, string responseMessage, string targetID);
}