using System.Windows;
using Caliburn.Micro;
using MD.Common.SoftwareStateHandling;

namespace MD.ChessCoach.UI.Windows.ViewModels;

public class ShellViewModel : Conductor<object>
{
    public ShellViewModel(IScreenManager screenManager)
    {
        screenManager.SubscribeToScreenChange(ChangeActiveItem);

        SoftwareStateManager softwareStateManager = SoftwareStateManager.Instance;

        softwareStateManager.SetExitHandler(ExitHandler);
    }

    private async void ExitHandler()
    {
        await TryCloseAsync();
        Application.Current.Shutdown();
    }

    private void ChangeActiveItem(Screen screen)
    {
        ActiveItem = screen;
    }
}