namespace YetAnotherHttpServerFramework
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class HttpResponse : IPrintable
    {
        private int _statusCode;        
        private Dictionary<string, string> _headers = new();
        private string _reason;
        private string _body;

        public HttpResponse() { }
        public HttpResponse UseStatusCode(int statusCode)
        {
            _statusCode = statusCode;
            return this;
        }
        public HttpResponse UseReason(string reason)
        {
            _reason = reason;
            return this;
        }
        public HttpResponse UseBody(string body)
        {
            _body = body;
            return this;
        }
        public HttpResponse UseHeader(string headerName, string headerValue)
        {
            _headers[headerName] = headerValue;
            return this;
        }
        public string Print()
        {
            StringBuilder sb = new();
            sb.AppendLine($"HTTP/1.1 {_statusCode} {_reason}");
            sb.AppendLine( string.Join("\r\n", _headers.Select(x => $"{_headers.Keys}: {_headers.Values}")));
            sb.AppendLine("\r\n");
            sb.AppendLine(_body);
            return sb.ToString();
        }
    }
}
