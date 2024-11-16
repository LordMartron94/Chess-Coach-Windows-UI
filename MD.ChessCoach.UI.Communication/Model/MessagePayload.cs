using System.Diagnostics.CodeAnalysis;

namespace MD.ChessCoach.UI.Communication.Model;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct MessagePayload
{
    public string action { get; set; }
    public Argument[] args { get; set; }

    public override string ToString()
    {
        string argsString = args.Aggregate("", (current, arg) => current + $"{arg.ToString()}, ");

        return $"Action: {action}, Args: {argsString}";
    }
}