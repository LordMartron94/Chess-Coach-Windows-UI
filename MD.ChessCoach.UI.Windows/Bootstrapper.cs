using System.Windows;
using Caliburn.Micro;
using MD.ChessCoach.UI.Communication;
using MD.ChessCoach.UI.Windows.Library;
using MD.ChessCoach.UI.Windows.ViewModels;
using MD.Common.SoftwareStateHandling;
using MD.Common.Utils;
using LogManager = MD.Common.Utils.LogManager;

namespace MD.ChessCoach.UI.Windows;

public class Bootstrapper : BootstrapperBase
{
    private readonly API? _api;
    private readonly SimpleContainer _container;
    private readonly HoornLogger _logger;
    private readonly SoftwareStateManager? _softwareStateManager;


    public Bootstrapper()
    {
        _container = new SimpleContainer();
        
        string? value = Environment.GetEnvironmentVariable("Apprun");

        if (value is not "true")
            return;
        
        _api = API.Instance;
        _softwareStateManager = SoftwareStateManager.Instance;
        _logger = HoornLogger.Instance;
        _softwareStateManager.SubscribeToState(State.Shutdown, _api.Shutdown);

        Initialize();
    }

    protected override void Configure()
    {
        if (_api!= null)
            _logger.Initialize(_api);
        
        LogManager.Instance.Initialize(_logger);

        _logger.Debug("Configuring Caliburn Micro.", moduleSeparator: "Bootstrapper");

        _container.Instance(_container);

        _container
            .Singleton<IWindowManager, WindowManager>()
            .Singleton<IEventAggregator, EventAggregator>()
            .Singleton<IScreenManager, ScreenManager>();

        GetType().Assembly.GetTypes()
            .Where(type => type.IsClass)
            .Where(type => type.Name.EndsWith("ViewModel"))
            .ToList()
            .ForEach(viewModelType => _container.RegisterPerRequest(
                viewModelType, viewModelType.ToString(), viewModelType));

        _container.GetInstance<IScreenManager>().Initialize(_container, _logger);

        _logger.Debug("Caliburn Micro configured.", moduleSeparator: "Bootstrapper");
    }

    [Attributes.TimeExecution]
    protected override void OnStartup(object sender, StartupEventArgs e)
    {
        _logger.Debug("Starting Bootstrapper.", moduleSeparator: "Bootstrapper");

        DisplayRootViewForAsync<ShellViewModel>();
        IScreenManager? screenManager = _container.GetInstance<IScreenManager>();
        screenManager.ChangeScreen(AppScreen.HomeScreen);

        _logger.Info("Bootstrapper started.", moduleSeparator: "Bootstrapper");
        _logger.Info("Homescreen launched successfully.", moduleSeparator: "Bootstrapper");
    }

    protected override void OnExit(object sender, EventArgs e)
    {
        _softwareStateManager.Shutdown();
    }


    protected override object GetInstance(Type service, string key)
    {
        return _container.GetInstance(service, key);
    }

    protected override IEnumerable<object> GetAllInstances(Type service)
    {
        return _container.GetAllInstances(service);
    }

    protected override void BuildUp(object instance)
    {
        _container.BuildUp(instance);
    }
}