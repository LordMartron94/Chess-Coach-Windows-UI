using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace MD.ChessCoach.UI.Communication._Internal.Connectivity;

/// <summary>
///     Handles any exceptions that occur during communication with the server.
/// </summary>
public class ConnectionExceptionHandler
{
    private void LogException(Exception ex, string methodName, string description)
    {
        Console.WriteLine($"{description} | Method: {methodName} | Message: {ex.Message}");
    }

    public void HandleException(Exception ex, [CallerMemberName] string? methodName = null)
    {
        switch (ex)
        {
            case IOException:
                LogException(ex, methodName!, "An IO Error occurred while communicating with the server.");
                break;
            case HttpRequestException:
                LogException(ex, methodName!, "An HTTP request error occurred while communicating with the server.");
                break;
            case WebException:
                LogException(ex, methodName!, "A web error occurred while communicating with the server.");
                break;
            case SocketException:
                LogException(ex, methodName!, "A socket error occurred while communicating with the server.");
                break;
        }
    }
}