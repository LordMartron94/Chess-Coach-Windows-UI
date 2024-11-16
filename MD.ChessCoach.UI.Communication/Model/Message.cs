using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace MD.ChessCoach.UI.Communication.Model;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal struct JsonMessage
{
    public string sender_id { get; set; }
    public DateTime time_sent { get; set; }
    public MessagePayload payload { get; set; } 
    public string unique_id { get; set; }
    public string? target_id { get; set; }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct Message
{
    public string sender_id { get; } = "5b4b25d1-9538-40ce-8147-4d712013543e";
    public DateTime time_sent { get; } = DateTime.Now;
    public MessagePayload payload { get; set; } 
    public string unique_id { get; } = Guid.NewGuid().ToString();
    public string? target_id { get; } = null;

    // Private constructor to initialize all fields
    private Message(string senderId, DateTime timeSent, MessagePayload payload, string uniqueId, string? targetId)
    {
        sender_id = senderId;
        time_sent = timeSent;
        this.payload = payload;
        unique_id = uniqueId;
        target_id = targetId;
    }

    public Message(MessagePayload payload)
    {
        this.payload = payload;
    }
    
    public Message(MessagePayload payload, string targetId)
    {
        this.payload = payload;
        target_id = targetId;
    }

    public static Message FromJson(string jsonString)
    {
        JsonMessage message = JsonConvert.DeserializeObject<JsonMessage>(jsonString);

        return new Message(message.sender_id, message.time_sent, message.payload, message.unique_id, message.target_id);
    }

    public override string ToString()
    {
        string messageString = $"Message ID: {unique_id}\nTime Sent: {time_sent}\nSender ID: {sender_id}\nTarget ID: {target_id}\nPayload:\n{payload.ToString()}";
        return messageString;
    }
}