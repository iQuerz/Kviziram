public class KviziramException : Exception
{
    public KviziramException(): base() { }
    public KviziramException(string message): base(message) { }
    public KviziramException(string message, Exception inner): base(message, inner) {}    
}
