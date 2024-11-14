using System.Net;
using System.Net.Sockets;
using System.Text;
using MD.ChessCoach.UI.Communication._Internal.Processing;
using MD.ChessCoach.UI.Communication._Internal.Scanning;
using MD.ChessCoach.UI.Communication.Model;
using Newtonsoft.Json;

namespace MD.ChessCoach.UI.Communication._Internal.Connectivity;

/// <summary>
///     Low-Level API for connecting with remote hosts.
/// </summary>
public class Connector
{
    private readonly string _endOfMessageToken;
    private readonly CancellationTokenSource _cancellationTokenSource; 

    private readonly IScanner _scanner;

    public Connector(IMessageProcessor messageProcessor, string endOfMessageToken = "<eom>")
    {
        _scanner = new Scanner(messageProcessor);
        
        _endOfMessageToken = endOfMessageToken;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Shutdown()
    {
        _cancellationTokenSource.Cancel();
    }

    /// <summary>
    ///     Connects to a remote host and port using TCP and continuously listens for data.
    ///     Args:
    ///     host: The hostname or IP address of the remote host.
    ///     port: The port number on the remote host.
    ///     componentPort: The port number to bind the socket to. Defaults to 5555.
    ///     Returns:
    ///     The connected socket, or null on failure. Logs error messages on failure.
    /// </summary>
    public Socket? ConnectToRemote(string host, int port, int componentPort = 6666)
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        s.Bind(new IPEndPoint(IPAddress.Parse(host), componentPort));
        s.SetSocketOption(SocketOptionLevel.Socket,  
            SocketOptionName.ReuseAddress, true); // Allow port reuse

        try
        {
            s.Connect(new IPEndPoint(IPAddress.Parse(host), port));
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionRefused)
        {
            Console.WriteLine($"Connection to {host}:{port} refused.");
            return null;
        }

        Thread thread = new Thread(() => ReadDataLoop(s, host, port));
        thread.Start();

        return s;
    }

    /// <summary>
    /// Initiates a background thread that sends periodic keep-alive messages to maintain the connection.
    /// </summary>
    /// <param name="s">The connected socket through which keep-alive messages will be sent.</param>
    /// <remarks>
    /// This method creates and starts a new thread that continuously sends keep-alive messages
    /// to the server at regular intervals. The keep-alive messages help prevent the connection
    /// from timing out due to inactivity.
    /// Skips the first interval because it's not necessary.
    /// </remarks>
    public void StartKeepAliveMessageLoop(Socket s)
    {
        Thread keepAliveThread = new Thread(() => SendKeepAliveMessage(s));
        keepAliveThread.Start();
    }

    /// <summary>
    /// Initiates a background thread that sends periodic keep-alive messages to maintain the connection.
    /// </summary>
    /// <param name="s">The connected socket through which keep-alive messages will be sent.</param>
    /// <remarks>
    /// This method creates and starts a new thread that continuously sends keep-alive messages
    /// to the server at regular intervals. The keep-alive messages help prevent the connection
    /// from timing out due to inactivity.
    /// Skips the first interval because it's not necessary.
    /// </remarks>
    private void SendKeepAliveMessage(Socket s)
    {
        const int keepAliveInterval = 30000; // 30 seconds
        
        Thread.Sleep(keepAliveInterval); // No need to send a keep-alive message right away.

        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            MessagePayload payload = PayloadFactory.BuildPayload(
                action: "keep_alive",
                args: new Dictionary<string, string>());
        
            Message message = new Message(payload);
            SendMessage(s, message);
            
            try
            {
                Thread.Sleep(keepAliveInterval);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    private void ReadDataLoop(Socket s, string host, int port)
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
            try
            {
                _scanner.Scan(s, _cancellationTokenSource, _endOfMessageToken);
            }
            catch (SocketException ex)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    Console.WriteLine($"Connection to {host}:{port} closed.");
                    break;
                }
                
                Console.WriteLine($"Error reading data from {host}:{port}: {ex.Message}\nClosing connection.");
                s.Close();
            }
    }
    
    public void SendMessage(Socket socket, Message message)
    {
        // Note to self: Do not use Time Execution Logging here, as it will cause infinite recursion (time execution -> log -> send message -> time execution)
        string messageSerialized = JsonConvert.SerializeObject(message);
        messageSerialized += _endOfMessageToken;
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageSerialized);
        
        socket.Send(messageBytes);
    }
}