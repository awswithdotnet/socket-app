using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DiskReader;

class Program
{
    static async Task Main(string[] args)
    {

        String terminationCode = "<|terminate|>";
        bool shouldLoop = true;

        var ipEndPoint = new IPEndPoint(IPAddress.Any, 11000);

        using Socket listener = new(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        listener.Bind(ipEndPoint);
        listener.Listen(100);

        while (shouldLoop)
        {
            Socket handler = await listener.AcceptAsync();

            // Receive message.
            var buffer = new byte[1_024];
            var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);

            if (response.IndexOf(terminationCode) > -1)
            {
                shouldLoop = false;
                await handler.DisconnectAsync(false);
                break;
            }

            String fileName = response.Trim();
            Console.WriteLine("File requested: " + fileName);

            byte[] fileBytes = await ReadFile(fileName);

            if (fileBytes.Length > 0)
            {

                await handler.SendAsync(fileBytes, 0);

                Console.WriteLine(fileName + " sent.");
            }

            await handler.DisconnectAsync(true);

        }

    }

    private static async Task<byte[]> ReadFile(string fileName)
    {

        if (File.Exists(fileName))
        {

            byte[] fileBytes = await File.ReadAllBytesAsync(fileName);

            return fileBytes;
        }

        return [];
    }
}