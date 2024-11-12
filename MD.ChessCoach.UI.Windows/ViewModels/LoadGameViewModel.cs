using Caliburn.Micro;

namespace MD.ChessCoach.UI.Windows.ViewModels;

public class LoadGameViewModel : Screen
{
    private readonly IScreenManager _screenManager;

    public LoadGameViewModel(IScreenManager screenManager)
    {
        _screenManager = screenManager;
    }

    public void Back()
    {
        _screenManager.ChangeScreen(AppScreen.HomeScreen);
    }
}