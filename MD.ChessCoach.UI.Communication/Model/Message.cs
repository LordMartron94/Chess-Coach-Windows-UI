using System.Diagnostics.CodeAnalysis;

namespace MD.ChessCoach.UI.Communication.Model;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct Message(MessagePayload payload)
{
    public string sender_id { get; } = "5b4b25d1-9538-40ce-8147-4d712013543e";
    public DateTime time_sent { get; set; } = DateTime.Now;
    public MessagePayload payload { get; set; } = payload;

    public string unique_id { get; } = Guid.NewGuid().ToString();
}