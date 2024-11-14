using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal;

internal static class PayloadFactory
{
    public static MessagePayload BuildPayload(string action, Dictionary<string, string> args)
    {
        Argument[] arguments = args.Select(arg => new Argument
        {
            type = arg.Key,
            value = arg.Value
        }).ToArray();
        
        MessagePayload payload = new MessagePayload
        {
            action = action,
            args = arguments
        };

        return payload;
    }
}