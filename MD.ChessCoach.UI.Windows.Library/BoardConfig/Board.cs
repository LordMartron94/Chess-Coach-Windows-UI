using MD.Common.Utils;

namespace MD.ChessCoach.UI.Windows.Library;

public class Board
{
    private readonly Square[] _squares;
    private readonly ILogger _logger;
    
    public Board()
    {
        _logger = HoornLogger.Instance;
        _logger.Info("Initializing board", moduleSeparator: "Board");
        
        _squares = new Square[64];

        InitializeBoard();
        InitializeDefaultPieces();
    }

    public Square[] GetSquares()
    {
        return _squares;
    }

    [Attributes.TimeExecution]
    private void InitializeBoard()
    {
        for (int rank = 0; rank < 8; rank++) // ranks 1 - 8
        {
            for (int file = 0; file < 8; file++) // files a - h
            {
                int squareIndex = Helpers.GetBoardIndex(rank, file);
                _squares[squareIndex] = new Square
                {
                    Rank = rank,
                    File = file,
                    Owner = SquareOwner.None,
                    Piece = SquarePiece.None
                };
            }
        }
    }

    [Attributes.TimeExecution]
    private void InitializeDefaultPieces()
    {
        _logger.Info("Setting Default Pieces for Board", moduleSeparator: "Board");
        string[] whitePawns =
        [
            "a2",
            "b2",
            "c2",
            "d2",
            "e2",
            "f2",
            "g2",
            "h2"
        ];

        string[] blackPawns =
        [
            "a7",
            "b7",
            "c7",
            "d7",
            "e7",
            "f7",
            "g7",
            "h7"
        ];
        
        foreach (string square in whitePawns)
            SetSquare(square, SquareOwner.White, SquarePiece.Pawn);
        
        foreach (string square in blackPawns)
            SetSquare(square, SquareOwner.Black, SquarePiece.Pawn);
        
        // White pieces
        SetSquare("a1", SquareOwner.White, SquarePiece.Rook);
        SetSquare("b1", SquareOwner.White, SquarePiece.Knight);
        SetSquare("c1", SquareOwner.White, SquarePiece.Bishop);
        SetSquare("d1", SquareOwner.White, SquarePiece.Queen);
        SetSquare("e1", SquareOwner.White, SquarePiece.King);
        SetSquare("f1", SquareOwner.White, SquarePiece.Bishop);
        SetSquare("g1", SquareOwner.White, SquarePiece.Knight);
        SetSquare("h1", SquareOwner.White, SquarePiece.Rook);
        
        // Black pieces
        SetSquare("a8", SquareOwner.Black, SquarePiece.Rook);
        SetSquare("b8", SquareOwner.Black, SquarePiece.Knight);
        SetSquare("c8", SquareOwner.Black, SquarePiece.Bishop);
        SetSquare("d8", SquareOwner.Black, SquarePiece.Queen);
        SetSquare("e8", SquareOwner.Black, SquarePiece.King);
        SetSquare("f8", SquareOwner.Black, SquarePiece.Bishop);
        SetSquare("g8", SquareOwner.Black, SquarePiece.Knight);
        SetSquare("h8", SquareOwner.Black, SquarePiece.Rook);
    }

    private void SetSquare(string square, SquareOwner owner, SquarePiece piece)
    {
        (int rank, int file) = Helpers.ParseSquare(square);
        int squareIndex = Helpers.GetBoardIndex(rank, file);
        
        _squares[squareIndex].Owner = owner;
        _squares[squareIndex].Piece = piece;
    }
}