using System.Diagnostics.CodeAnalysis;

namespace MD.ChessCoach.UI.Communication.Model;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct Argument
{
    public string type { get; set; }
    public string? value { get; set; }

    public override string ToString()
    {
        return $"type: {type}, value: {value}";
    }
}