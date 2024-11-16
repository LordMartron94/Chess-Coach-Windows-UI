using System.Net.Sockets;
using System.Text;
using MD.ChessCoach.UI.Communication._Internal.Processing;
using MD.ChessCoach.UI.Communication.Model;
using Newtonsoft.Json;

namespace MD.ChessCoach.UI.Communication._Internal.Scanning;

internal class Scanner : IScanner
{
    private readonly IMessageProcessor _messageProcessor;
    
    public Scanner(IMessageProcessor messageProcessor)
    {
        _messageProcessor = messageProcessor;
    }
    
    public byte[] Scan(Socket socket, CancellationTokenSource cancellationTokenSource, string delimiter = "\n")
    {
        byte[] buffer = Array.Empty<byte>();
        
        byte[] data = new byte[4096];
        int bytesRead = socket.Receive(data);
        cancellationTokenSource.Token.ThrowIfCancellationRequested(); 

        if (bytesRead == 0)
            return buffer;

        buffer = Combine(buffer, data, bytesRead);

        byte[] eomTokenBytes = Encoding.ASCII.GetBytes(delimiter);

        if (buffer.AsSpan().IndexOf(eomTokenBytes) == -1)
            return buffer;

        byte[][] parts = Split(buffer, eomTokenBytes);
        string message = Encoding.ASCII.GetString(parts[0]);
        ProcessMessage(message);
        buffer = parts[1];

        return buffer;
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
    
    private void ProcessMessage(string message)
    {
        // Console.WriteLine($"Received message: {message}");
        
        Message processedMessage = Message.FromJson(message);
        _messageProcessor.ProcessIncomingMessage(processedMessage);
    }
}