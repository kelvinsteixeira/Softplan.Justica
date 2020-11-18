using System.Collections.Generic;

namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces
{
    public interface INotificationContext
    {
        IReadOnlyCollection<Notification> Notifications { get; }

        bool HasNotifications { get; }

        void Add(Notification notification);

        void Add(string key, string message);
    }
}