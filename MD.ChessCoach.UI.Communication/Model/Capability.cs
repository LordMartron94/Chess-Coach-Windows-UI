using System.Diagnostics.CodeAnalysis;

namespace MD.ChessCoach.UI.Communication.Model;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct Capability
{
    public string name { get; set; }
    public Signature signature { get; set; }
}