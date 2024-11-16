namespace MD.ChessCoach.UI.Windows.Library;

public struct Square
{
    public int Rank { get; set; }
    public int File { get; set; }
    
    public SquareOwner Owner { get; set; }
    public SquarePiece Piece { get; set; }
}