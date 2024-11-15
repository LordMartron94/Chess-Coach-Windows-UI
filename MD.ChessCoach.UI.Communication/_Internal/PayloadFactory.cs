using MD.ChessCoach.UI.Communication.Model;

namespace MD.ChessCoach.UI.Communication._Internal;

internal static class PayloadFactory
{
    public static MessagePayload BuildPayload(string action, IEnumerable<(string, string)> args)
    {
        Argument[] arguments = args.Select(arg => new Argument
        {
            type = arg.Item1,
            value = arg.Item2
        }).ToArray();
        
        MessagePayload payload = new MessagePayload
        {
            action = action,
            args = arguments
        };

        return payload;
    }
}