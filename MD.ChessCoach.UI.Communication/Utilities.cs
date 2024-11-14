using System.Reflection;
using MD.ChessCoach.UI.Communication.Model;
using Newtonsoft.Json;

namespace MD.ChessCoach.UI.Communication;

public static class Utilities
{
    public static Message GetMessageFromSharedResource(string resourceName, MessagePayload? messagePayload = null)
    {
        // Note to self: Do not use Time Execution Logging here, as it will cause infinite recursion (time execution -> log -> send message -> time execution)

        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException($"{resourceName} JSON not included!");
        using StreamReader reader = new StreamReader(stream);

        Message message = JsonConvert.DeserializeObject<Message>(reader.ReadToEnd());
        message = new Message(message.payload);

        if (messagePayload != null)
            message.payload = messagePayload.Value;

        return message;
    }

    /// <summary>
    /// Builds a MessagePayload object with the given action.
    /// Mostly used for default special actions (shutdown, unregister, keep_alive).
    /// </summary>
    /// <param name="action">The associated parameterless action.</param>
    /// <param name="args">Optional argument specification.</param>
    /// <returns>MessagePayload object</returns>
    public static MessagePayload BuildPayload(string action, Argument[]? args = null)
    {
        Argument[] arguments = args ?? Array.Empty<Argument>();
        
        return new MessagePayload
        {
            action = action,
            args = arguments
        };
    }
}