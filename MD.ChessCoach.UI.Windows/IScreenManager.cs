using Caliburn.Micro;
using MD.ChessCoach.UI.Windows.Library;

namespace MD.ChessCoach.UI.Windows;

public interface IScreenManager
{
    void Initialize(SimpleContainer container, HoornLogger logger);

    Screen? GetCurrentScreen();

    void ChangeScreen(AppScreen screen);

    void SubscribeToScreenChange(Action<Screen> callback);

    void UnsubscribeFromScreenChange(Action<Screen> callback);
}