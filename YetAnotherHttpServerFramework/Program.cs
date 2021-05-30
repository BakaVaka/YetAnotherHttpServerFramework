namespace YetAnotherHttpServerFramework
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main()
        {
            HttpServer server = new HttpServer(new IPEndPoint(IPAddress.Any, 8080), 1024);
            await server.RunAsync();
        }
    }
}
