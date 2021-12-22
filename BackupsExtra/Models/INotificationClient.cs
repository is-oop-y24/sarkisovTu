namespace BackupsExtra.Models
{
    public interface INotificationClient
    {
        void Update(NotificationMessage message);
    }
}