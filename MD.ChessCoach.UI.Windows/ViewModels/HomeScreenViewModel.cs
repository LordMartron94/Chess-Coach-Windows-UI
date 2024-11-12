using Caliburn.Micro;
using MD.ChessCoach.UI.Windows.Library;
using MD.Common.SoftwareStateHandling;

namespace MD.ChessCoach.UI.Windows.ViewModels;

public class HomeScreenViewModel : Screen
{
    public string GameName { get; set; }
    public string GameVersion { get; set; }
    private readonly IScreenManager _screenManager;
    private readonly SoftwareStateManager _softwareStateManager;

    public HomeScreenViewModel(IScreenManager screenManager)
    {
        GameName = Constants.GameName;
        GameVersion = "Version: " + Constants.GameVersion;
        
        _softwareStateManager = SoftwareStateManager.Instance;

        _screenManager = screenManager ?? throw new ArgumentNullException(nameof(screenManager));
    }

    public void StartNewGame()
    {
        _screenManager.ChangeScreen(AppScreen.NewGameScreen);
    }

    public void LoadGame()
    {
        _screenManager.ChangeScreen(AppScreen.LoadGameScreen);
    }

    public void Exit()
    {
        _softwareStateManager.Shutdown();
    }

    public void Credits()
    {
        _screenManager.ChangeScreen(AppScreen.CreditsScreen);
    }

    public void Options()
    {
        _screenManager.ChangeScreen(AppScreen.OptionsScreen);
    }
}