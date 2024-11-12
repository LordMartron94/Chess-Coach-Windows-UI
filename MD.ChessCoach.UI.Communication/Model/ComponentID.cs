using System.Diagnostics.CodeAnalysis;

namespace MD.ChessCoach.UI.Communication.Model;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct ComponentID
{
    public string title { get; set; }
    public string version { get; set; }
    public Capability[] capabilities { get; set; }
}