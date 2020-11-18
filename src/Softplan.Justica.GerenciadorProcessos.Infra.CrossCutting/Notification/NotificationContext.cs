using System.Collections.Generic;
using System.Linq;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification
{
    public class NotificationContext : INotificationContext
    {
        private readonly List<Notification> internalNotifications;

        public NotificationContext()
        {
            this.internalNotifications = new List<Notification>();
        }

        public IReadOnlyCollection<Notification> Notifications => this.internalNotifications;

        public bool HasNotifications => this.internalNotifications.Any();

        public void Add(Notification notification)
        {
            this.internalNotifications.Add(notification);
        }

        public void Add(string key, string message)
        {
            this.internalNotifications.Add(new Notification(key, message));
        }
    }
}