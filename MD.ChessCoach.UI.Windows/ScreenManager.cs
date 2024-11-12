using Caliburn.Micro;
using MD.ChessCoach.UI.Windows.Library;
using MD.ChessCoach.UI.Windows.ViewModels;

namespace MD.ChessCoach.UI.Windows;

/// <summary>
///     Provides screen switching functionality for use with Caliburn Micro.
/// </summary>
/// <remarks>
///     Does no actual displaying on its own... This must be done through subscriptions. For example with a Conductor.
///     <br />This is for optimal versatility.
/// </remarks>
public sealed class ScreenManager : IScreenManager
{
    private Screen? _currentScreen;

    private bool _initialized;
    private HoornLogger? _logger;

    private readonly List<Action<Screen>> _screenChangeSubscriptions;
    private Dictionary<AppScreen, Screen?>? _screens;

    public ScreenManager()
    {
        _screenChangeSubscriptions = new List<Action<Screen>>();
        _initialized = false;
    }

    public void Initialize(SimpleContainer container, HoornLogger logger)
    {
        if (_initialized)
        {
            _logger?.Warning("ScreenManager has already been initialized. Ignoring second initialization.", moduleSeparator: "ScreenManager");
            return;
        }

        _initialized = true;

        _logger = logger;

        _screens = new Dictionary<AppScreen, Screen?>
        {
            { AppScreen.HomeScreen, container.GetInstance<HomeScreenViewModel>() },
            { AppScreen.CreditsScreen, container.GetInstance<CreditsScreenViewModel>() },
            { AppScreen.LoadGameScreen, container.GetInstance<LoadGameViewModel>() },
            { AppScreen.NewGameScreen, container.GetInstance<NewGameViewModel>() },
            { AppScreen.OptionsScreen, container.GetInstance<OptionsViewModel>() }
        };
    }

    public void SubscribeToScreenChange(Action<Screen> action)
    {
        _logger?.Debug($"Subscribed to screen change event for action {action.Method.Name}");

        if (!_screenChangeSubscriptions.Contains(action))
            _screenChangeSubscriptions.Add(action);
    }

    public void UnsubscribeFromScreenChange(Action<Screen> action)
    {
        _logger?.Debug($"Unsubscribed from screen change event for action {action.Method.Name}");

        _screenChangeSubscriptions.Remove(action);
    }

    public Screen? GetCurrentScreen()
    {
        _logger?.Debug($"Returning current screen. Current screen: {_currentScreen?.GetType().Name} or null if none.");
        return _currentScreen;
    }

    public async void ChangeScreen(AppScreen screen)
    {
        if (!_initialized)
        {
            _logger?.Error("ScreenManager has not been initialized. Initialize it before calling ChangeScreen.", moduleSeparator: "ScreenManager");
            return;
        }

        _logger?.Debug($"Changing screen to {screen.ToString()}");

        // Deactivate the current screen first, if any. May be redundant if used with a Conductor, but we keep it here for completeness.
        // Just in case the developer doesn't use a Conductor.
        if (_currentScreen != null)
        {
            _logger?.Debug($"Deactivating current screen {_currentScreen?.GetType().Name}");
            await _currentScreen.DeactivateAsync(false);
        }

        if (!_screens!.TryGetValue(screen, out Screen? screenInstance))
        {
            _logger?.Error($"Screen '{screen}' not found in the available screens.", moduleSeparator: "ScreenManager");
            return;
        }

        _currentScreen = screenInstance;

        foreach (Action<Screen> action in _screenChangeSubscriptions)
        {
            _logger?.Debug($"Invoking screen change event for action {action.Method.Name}");
            action.Invoke(screenInstance!);
        }

        // Same as with Deactivation.
        _logger?.Debug($"Activating new screen {_currentScreen?.GetType().Name}");
        await _currentScreen.ActivateAsync();
    }
}