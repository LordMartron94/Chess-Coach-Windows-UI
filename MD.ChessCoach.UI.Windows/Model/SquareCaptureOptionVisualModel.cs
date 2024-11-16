using System.Globalization;

namespace MD.ChessCoach.UI.Windows.Model;

public struct SquareCaptureOptionVisualModel
{
    public string ImagePath => "pack://application:,,,/Assets/Move Option Capture.png";
    public double XPosition { get; set; }
    public double YPosition { get; set; }

    public string Margin => $"{XPosition.ToString(NumberFormatInfo.CurrentInfo).Replace(",", ".")}, {YPosition.ToString(NumberFormatInfo.CurrentInfo).Replace(",", ".")}, 0, 0";
    
    public bool Active { get; set; }
}