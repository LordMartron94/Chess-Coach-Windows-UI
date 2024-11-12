using Caliburn.Micro;
using MD.ChessCoach.UI.Windows.Library;
using MD.ChessCoach.UI.Windows.Model;
using MD.Common.Utils;

namespace MD.ChessCoach.UI.Windows.ViewModels;

public class NewGameViewModel : Screen
{
    private readonly IScreenManager _screenManager;
    private readonly ILogger _logger;
    public List<SquareVisualModel> BoardPieces { get; set; } = [];
    
    public string Player1Name { get; set; }
    public string Player2Name { get; set; }
    
    private readonly Board _board;
    

    public NewGameViewModel(IScreenManager screenManager)
    {
        _screenManager = screenManager;
        _logger = HoornLogger.Instance;
        
        _logger.Debug("Initializing New Game View Model");
        _board = new Board();
        
        Player1Name = "Player 1";
        Player2Name = "Player 2";
        
        Square[] squares = _board.GetSquares();

        foreach (Square square in squares)
        {
            if (square.Owner == SquareOwner.None || square.Piece == SquarePiece.None)
                continue;
            
            _logger.Debug($"Adding piece to board: {square.Piece} at ({square.File}, {square.Rank}) - {Helpers.FormatSquare(square)} | Owner - {square.Owner}");
            
            (double x, double y) = Helpers.GetSquarePosition(square.File, square.Rank);
            
            BoardPieces.Add(new SquareVisualModel
            {
                ImagePath = "pack://application:,,,/Assets/Chess Set 1/" + Helpers.GetNameForPieceImage(square),
                XPosition = x,
                YPosition = y
            });
        }
        
        _logger.Debug("New Game View Model initialized successfully");
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

    private void MirrorPositionsVertically()
    {
        List<SquareVisualModel> mirroredPieces = BoardPieces.Select(piece => piece with { YPosition = Helpers.GetMirroredYPosition(piece.YPosition) }).ToList();
        BoardPieces = mirroredPieces;
        NotifyOfPropertyChange(nameof(BoardPieces));
    }
}