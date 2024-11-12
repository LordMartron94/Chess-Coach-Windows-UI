using System.Globalization;

namespace MD.ChessCoach.UI.Windows.Model;

public struct SquareVisualModel
{
    public string ImagePath { get; set; }
    public double XPosition { get; set; }
    public double YPosition { get; set; }

    public string Margin => $"{XPosition.ToString(NumberFormatInfo.CurrentInfo).Replace(",", ".")}, {YPosition.ToString(NumberFormatInfo.CurrentInfo).Replace(",", ".")}, 0, 0";
}