using System;

public interface INotificationView
{
    event Action Opened;
    event Action Closed;

    void Initialize(NotificationData data, params object[] args);

    void Close();
}
