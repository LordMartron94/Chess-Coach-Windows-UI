using System.Net.Sockets;

namespace MD.ChessCoach.UI.Communication._Internal.Scanning;

/// <summary>
/// Defines the interface for scanning a socket.
/// </summary>
public interface IScanner
{
    /// <summary>
    /// Scans the given socket and returns the scanned data.
    /// </summary>
    /// <param name="socket">The socket to scan.</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <param name="delimiter">The delimiter to split on. Defaults to newline.</param>
    /// <returns></returns>
    byte[] Scan(Socket socket, CancellationTokenSource cancellationTokenSource, string delimiter = "\n");
}