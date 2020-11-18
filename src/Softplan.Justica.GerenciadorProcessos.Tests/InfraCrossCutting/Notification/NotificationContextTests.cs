using FluentAssertions;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.InfraCrossCutting.Notification
{
    public class NotificationContextTests
    {
        [Fact]
        public void ValidarNotificationContextComNotificacao()
        {
            // Arrange
            var sut = new NotificationContext();

            // Act
            sut.Add(new Infra.CrossCutting.Notification.Notification("Key1", "Message1"));
            sut.Add("Key2", "Message2");

            // Assert
            sut.HasNotifications.Should().BeTrue();
            sut.Notifications.Should().HaveCount(2);
            sut.Notifications.Should().Contain(n => n.Key == "Key1" && n.Message == "Message1");
            sut.Notifications.Should().Contain(n => n.Key == "Key2" && n.Message == "Message2");
        }

        [Fact]
        public void ValidarNotificationContextSemNotificacao()
        {
            // Arrange
            var sut = new NotificationContext();

            // Act
            // Assert
            sut.HasNotifications.Should().BeFalse();
        }
    }
}