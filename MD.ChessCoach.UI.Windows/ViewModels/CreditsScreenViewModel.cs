using Caliburn.Micro;

namespace MD.ChessCoach.UI.Windows.ViewModels;

public class CreditsScreenViewModel : Screen
{
    private readonly IScreenManager _screenManager;

    public CreditsScreenViewModel(IScreenManager screenManager)
    {
        _screenManager = screenManager;
    }

    public void Back()
    {
        _screenManager.ChangeScreen(AppScreen.HomeScreen);
    }
}