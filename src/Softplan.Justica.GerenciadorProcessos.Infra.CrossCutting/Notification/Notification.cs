namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification
{
    public class Notification
    {
        public Notification(string key, string message)
        {
            this.Key = key;
            this.Message = message;
        }

        public string Key { get; }

        public string Message { get; }
    }
}