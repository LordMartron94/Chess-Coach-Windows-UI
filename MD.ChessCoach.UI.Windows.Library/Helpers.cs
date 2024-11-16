namespace MD.ChessCoach.UI.Windows.Library;

public static class Helpers
{
    private readonly static Dictionary<int, string> FileNotations = new Dictionary<int, string>
    {
        { 0, "a" },
        { 1, "b" },
        { 2, "c" },
        { 3, "d" },
        { 4, "e" },
        { 5, "f" },
        { 6, "g" },
        { 7, "h" }
    };

    /// <summary>
    /// Converts a square to a string representation in algebraic notation.
    /// </summary>
    /// <param name="square"></param>
    /// <returns></returns>
    public static string FormatSquare(Square square)
    {
        string fileRepresentation = FileNotations[square.File];
        string rankRepresentation = (square.Rank + 1).ToString();
        return $"{fileRepresentation}{rankRepresentation}";
    }

    /// <summary>
    /// Converts a string representation in algebraic notation to a square.
    /// </summary>
    /// <param name="squareNotation"></param>
    /// <returns>The square representation as rank, file</returns>
    public static (int, int) ParseSquare(string squareNotation)
    {
        int fileIndex = FileNotations.FirstOrDefault(x => x.Value == squareNotation[0].ToString()).Key;
        int rank = int.Parse(squareNotation[1].ToString()) - 1;
        return (rank, fileIndex);
    }

    public static int GetBoardIndex(int rank, int file)
    {
        return rank * 8 + file;
    }

    public static string GetNameForPieceImage(Square square)
    {
        if (square.Owner == SquareOwner.White)
        {
            return square.Piece switch
            {
                SquarePiece.Pawn => "White_Pawn.png",
                SquarePiece.Rook => "White_Rook.png",
                SquarePiece.Knight => "White_Knight.png",
                SquarePiece.Bishop => "White_Bishop.png",
                SquarePiece.Queen => "White_Queen.png",
                SquarePiece.King => "White_King.png",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return square.Piece switch
        {
            SquarePiece.Pawn => "Black_Pawn.png",
            SquarePiece.Rook => "Black_Rook.png",
            SquarePiece.Knight => "Black_Knight.png",
            SquarePiece.Bishop => "Black_Bishop.png",
            SquarePiece.Queen => "Black_Queen.png",
            SquarePiece.King => "Black_King.png",
            _ => throw new ArgumentOutOfRangeException()
        };

    }

    public static (double, double) GetSquarePosition(int file, int rank, double width = 81.25, double height = 81.25)
    {
        double x = file * width;
        double y = (7 - rank) * height;
        
        return (x, y);
    }

    /// <summary>
    /// Mirrors the Y position vertically.
    /// </summary>
    /// <param name="y"></param>
    /// <param name="height"></param>
    /// <returns>The y position of the square but from the opposite end</returns>
    public static double GetMirroredYPosition(double y, double height = 81.25)
    {
        // ReSharper disable once ArrangeRedundantParentheses
        return (height * 7) - y;
    }
}