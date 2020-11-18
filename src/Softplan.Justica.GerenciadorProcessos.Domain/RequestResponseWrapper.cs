namespace Softplan.Justica.GerenciadorProcessos.Domain
{
    public interface IRequestResponseWrapper
    {
        bool Success { get; }
    }

    public class RequestResponseWrapper : IRequestResponseWrapper
    {
        public RequestResponseWrapper(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
    }

    public class RequestResponseWrapper<T> : RequestResponseWrapper
    {
        public RequestResponseWrapper(bool success, T value) : base(success)
        {
            Value = value;
        }

        public T Value { get; }
    }
}