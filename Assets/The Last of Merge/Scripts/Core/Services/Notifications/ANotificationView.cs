using System;
using UnityEngine;

public abstract class ANotificationView : MonoBehaviour, INotificationView
{
    public event Action Opened;
    public event Action Closed;

    public virtual void Close()
    {
        Closed?.Invoke();
    }

    public virtual void Initialize(NotificationData data, params object[] args)
    {
        Opened?.Invoke();
    }
}
