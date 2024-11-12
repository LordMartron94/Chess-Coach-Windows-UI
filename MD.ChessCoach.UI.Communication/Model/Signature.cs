using System.Diagnostics.CodeAnalysis;

namespace MD.ChessCoach.UI.Communication.Model;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct Signature
{
    public int num_of_args { get; set; }
    public string[] type_of_args { get; set; }
}