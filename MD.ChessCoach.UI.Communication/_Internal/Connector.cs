using System.Net;
using System.Net.Sockets;
using System.Text;
using MD.ChessCoach.UI.Communication.Model;
using Newtonsoft.Json;

namespace MD.ChessCoach.UI.Communication._Internal;

/// <summary>
///     Low-Level API for connecting with remote hosts.
/// </summary>
public class Connector
{
    private readonly string _endOfMessageToken;
    private readonly CancellationTokenSource _cancellationTokenSource; 

    private readonly Action<MessagePayload> _messageReceivedListener;

    public Connector(Action<MessagePayload> messageReceivedListener, string endOfMessageToken = "<eom>")
    {
        _messageReceivedListener = messageReceivedListener;
        
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

    public void StartKeepAliveMessageLoop(Socket s)
    {
        Thread keepAliveThread = new Thread(() => SendKeepAliveMessage(s));
        keepAliveThread.Start();
    }

    private void SendKeepAliveMessage(Socket s)
    {
        const int keepAliveInterval = 30000; // 30 seconds

        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            MessagePayload payload = Utilities.BuildPayload("keep_alive");

            Message transformedMessage = Utilities.GetMessageFromSharedResource(Constants.DefaultRequestResourcePath, payload);
            SendMessage(s, transformedMessage);
            Thread.Sleep(keepAliveInterval);
        }
    }

    private void ReadDataLoop(Socket s, string host, int port)
    {
        byte[] buffer = Array.Empty<byte>();

        while (!_cancellationTokenSource.IsCancellationRequested)
            try
            {
                byte[] data = new byte[4096];
                int bytesRead = s.Receive(data);
                _cancellationTokenSource.Token.ThrowIfCancellationRequested(); 

                if (bytesRead == 0)
                    break;

                buffer = Combine(buffer, data, bytesRead);

                byte[] eomTokenBytes = Encoding.ASCII.GetBytes(_endOfMessageToken);

                if (buffer.AsSpan().IndexOf(eomTokenBytes) != -1)
                {
                    byte[][] parts = Split(buffer, eomTokenBytes);
                    string message = Encoding.ASCII.GetString(parts[0]);
                    _process_message(message, host, port);
                    buffer = parts[1];
                }
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
                break;
            }
    }

    // Helper functions for byte array manipulation
    private static byte[] Combine(byte[] first, byte[] second, int secondLength)
    {
        byte[] ret = new byte[first.Length + secondLength];
        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, secondLength);
        return ret;
    }

    private static byte[][] Split(byte[] source, byte[] delimiter)
    {
        int index = source.AsSpan().IndexOf(delimiter);

        if (index == -1)
            return [source, Array.Empty<byte>()];

        byte[] first = source.AsSpan(0, index).ToArray();
        byte[] second = source.AsSpan(index + delimiter.Length).ToArray();
        return [first, second];
    }

    private void _process_message(string message, string host, int port)
    {
        Console.WriteLine($"Received from {host}:{port}: {message}");
        Message processedMessage = JsonConvert.DeserializeObject<Message>(message);
        _messageReceivedListener(processedMessage.payload);
    }
    
    public void SendMessage(Socket socket, Message message)
    {
        // Note to self: Do not use Time Execution Logging here, as it will cause infinite recursion (time execution -> log -> send message -> time execution)
        string messageSerialized = JsonConvert.SerializeObject(message);
        messageSerialized += _endOfMessageToken;
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageSerialized);
        
        Console.WriteLine($"Sending {messageSerialized}");
        
        socket.Send(messageBytes);
    }
}