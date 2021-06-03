using System.Collections.Generic;

namespace YetAnotherHttpServerFramework
{
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