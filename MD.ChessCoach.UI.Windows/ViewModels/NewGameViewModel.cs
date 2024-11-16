using System.Collections.ObjectModel;
using System.Windows.Documents;
using Caliburn.Micro;
using MD.ChessCoach.UI.Communication;
using MD.ChessCoach.UI.Windows.Library;
using MD.ChessCoach.UI.Windows.Model;
using MD.Common.Utils;

namespace MD.ChessCoach.UI.Windows.ViewModels;

public class NewGameViewModel : Screen
{
    private readonly IScreenManager _screenManager;
    private readonly ILogger _logger;
    private readonly API _api;
    
    public List<SquareVisualModel> BoardPieces { get; set; } = [];
    public ObservableCollection<SquareMoveOptionVisualModel> MoveOptions { get; set; } = [];
    public List<SquareCaptureOptionVisualModel> CaptureOptions { get; set; } = [];
    
    public string Player1Name { get; set; }
    public string Player2Name { get; set; }
    
    private readonly Board _board;

    private List<int> _currentlyActiveSelectionOptions;
    

    public NewGameViewModel(IScreenManager screenManager)
    {
        _currentlyActiveSelectionOptions = new List<int>();
        
        _screenManager = screenManager;
        _logger = HoornLogger.Instance;
        _api = API.Instance;
        
        _logger.Debug("Initializing New Game View Model");
        _board = new Board();
        
        Player1Name = "Player 1";
        Player2Name = "Player 2";
        
        Square[] squares = _board.GetSquares();

        foreach (Square square in squares)
        {
            (double x, double y) = Helpers.GetSquarePosition(square.File, square.Rank);
            MoveOptions.Add(new SquareMoveOptionVisualModel
            {
                XPosition = x,
                YPosition = y,
                Active = false
            });

            CaptureOptions.Add(new SquareCaptureOptionVisualModel
            {
                XPosition = x,
                YPosition = y,
                Active = false
            });
            
            if (square.Owner == SquareOwner.None || square.Piece == SquarePiece.None)
                continue;
            
            BoardPieces.Add(new SquareVisualModel
            {
                ImagePath = "pack://application:,,,/Assets/Chess Set 1/" + Helpers.GetNameForPieceImage(square),
                XPosition = x,
                YPosition = y
            });
        }
        
        _logger.Debug("New Game View Model initialized successfully");
    }

    [Attributes.TimeExecution]
    public void SelectPiece(SquareVisualModel source)
    {
        string square = Helpers.GetSquareNotationForPosition(source.XPosition, source.YPosition);
        _logger.Debug($"Selected piece: {square}");

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (int optionIndex in _currentlyActiveSelectionOptions)
            MoveOptions[optionIndex].Active = false; // Reset
        
        string[] moves = _api.GetLegalMoves(square);

        foreach (string move in moves)
        {
            (int rank, int file) = Helpers.ParseSquare(move);
            (double xPos, double yPos) = Helpers.GetSquarePosition(file, rank);

            SquareMoveOptionVisualModel squareMoveOptionVisualModel = MoveOptions.First(option => Math.Abs(option.XPosition - xPos) < 0.001 && Math.Abs(option.YPosition - yPos) < 0.001);
            squareMoveOptionVisualModel.Active = true;
            
            _currentlyActiveSelectionOptions.Add(MoveOptions.IndexOf(squareMoveOptionVisualModel));
        }
    }

    public void Back()
    {
        _screenManager.ChangeScreen(AppScreen.HomeScreen);
    }

    [Attributes.TimeExecution]
    public void Reverse()
    {
        _logger.Debug("Reversing board.");
        (Player1Name, Player2Name) = (Player2Name, Player1Name);
        NotifyOfPropertyChange(nameof(Player1Name));
        NotifyOfPropertyChange(nameof(Player2Name));
        
        MirrorPositionsVertically();
        
        _logger.Info($"Board reversed with names: Player 1: {Player1Name}, Player 2: {Player2Name}");
    }

    [Attributes.TimeExecution]
    private void MirrorPositionsVertically()
    {
        List<SquareVisualModel> mirroredPieces = BoardPieces.Select(piece => piece with { YPosition = Helpers.GetMirroredYPosition(piece.YPosition) }).ToList();
        BoardPieces = mirroredPieces;
        NotifyOfPropertyChange(nameof(BoardPieces));
    }
}