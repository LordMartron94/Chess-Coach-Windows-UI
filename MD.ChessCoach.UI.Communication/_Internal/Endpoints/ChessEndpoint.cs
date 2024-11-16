using MD.ChessCoach.UI.Communication.Model;
using Newtonsoft.Json;

namespace MD.ChessCoach.UI.Communication._Internal.Endpoints;

internal class ChessEndpoint : AbEndpoint
{
    private List<string> _legalMoves;

    public ChessEndpoint(IMessageHandler messageHandler) : base(messageHandler)
    {
        _legalMoves = new List<string>();
    }

    public string[] GetLegalMovesForSquare(string square)
    {
        _legalMoves.Clear();

        const string action = "get_legal_moves_for_square";
        MessageHandler.SendRequest(action, new[] { ("string", square) }, OnLegalMovesReceived);

        return _legalMoves.ToArray();
    }

    private void OnLegalMovesReceived(Message response)
    {
        string? legalMovesJson = response.payload.args[0].value;

        if (legalMovesJson != null)
        {
            string[]? legalMoves = JsonConvert.DeserializeObject<string[]>(legalMovesJson);
            legalMoves ??= Array.Empty<string>();
            _legalMoves.AddRange(legalMoves);
        }
    }
}