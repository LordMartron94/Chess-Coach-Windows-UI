using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MD.ChessCoach.UI.Windows.Model;

public class SquareMoveOptionVisualModel : INotifyPropertyChanged
{
    public string ImagePath => "pack://application:,,,/Assets/Move Option.png";
    public double XPosition { get; set; }
    public double YPosition { get; set; }

    public string Margin => $"{XPosition.ToString(NumberFormatInfo.CurrentInfo).Replace(",", ".")}, {YPosition.ToString(NumberFormatInfo.CurrentInfo).Replace(",", ".")}, 0, 0";
    
    private bool _active;

    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}