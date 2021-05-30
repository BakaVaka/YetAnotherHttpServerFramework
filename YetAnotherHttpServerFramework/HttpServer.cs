using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherHttpServerFramework
{
    public class HttpServer
    {
        private readonly IPEndPoint _ipEndPoint;
        private readonly int _serverTasksCount;

        public HttpServer(IPEndPoint ipEndPoint, int serverTaksCount = 1)
        {
            if (serverTaksCount < 1)
            {
                throw new ArgumentException("Invalid task count");
            }
            _ipEndPoint = ipEndPoint;
            _serverTasksCount = serverTaksCount;
        }

        public async Task RunAsync()
        {
            using Socket serverSocket = new(_ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            serverSocket.Bind(_ipEndPoint);
            serverSocket.Listen(_serverTasksCount);
            var serverTasks = Enumerable
                .Range(0, _serverTasksCount)
                .Select(x => AcceptLoop(serverSocket));
            await Task.WhenAll(serverTasks);
        }

        private async Task AcceptLoop(Socket serverSocket)
        {
            while (true)
            {
                var client = await serverSocket.AcceptAsync();
                try
                {
                    await ProcessClient(client);
                }
                catch (SocketException ex) { }
                catch (ObjectDisposedException) { }
                catch (IOException) { }
            }
        }

        private async Task ProcessClient(Socket client)
        {
            using NetworkStream clientStream = new(client);
            using StreamReader streamReader = new(clientStream);
            StringBuilder requestBuffer = new();
            do
            {
                var requestString = await streamReader.ReadLineAsync();
                if (requestString.Length == 0) { break; }
                requestBuffer.AppendLine(requestString);
            } while (true);

            var isValidRequest = TryParseRequest(requestBuffer, out var request);
            if (!isValidRequest)
            {
                var response1 = "HTTP/1.1 500 Internal";
                await client.SendAsync(Encoding.UTF8.GetBytes(response1), SocketFlags.None);
                client.Dispose();
                return;
            }

            var response = "HTTP/1.1 200 OK\r\n\r\nHello";
            await client.SendAsync(Encoding.UTF8.GetBytes(response), SocketFlags.None);
            client.Dispose();
        }

        private bool TryParseRequest(StringBuilder requestString, out HttpRequest request)
        {
            request = default;
            using StringReader stringReader = new StringReader(requestString.ToString());
            var startString = stringReader.ReadLine().Split(' ');
            request = new HttpRequest
            {
                Method = startString[0],
                Path = startString[1],
                Protocol = startString[2],
                Headers = ParseHeaders(stringReader)
            };
            return true;
        }

        private Dictionary<string,string> ParseHeaders(StringReader stringReader)
        {
            Dictionary<string, string> headers = new();
            while(true)
            {
                var str = stringReader.ReadLine();
                if (str is null || str.StartsWith("\r\n"))
                {
                    break;
                }
                var keyValue = str.Split(":", 2, StringSplitOptions.RemoveEmptyEntries);
                headers[keyValue[0]] = keyValue[1].Trim();
            }
            return headers;
        }
    }
    public class HttpRequest
    {
        public string Method { get; set; }
        public string Protocol { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new();
        public string Body { get; set; }
    }
}