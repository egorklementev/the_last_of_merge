using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class NotificationViewProvider
{
    [Inject]
    private List<NotificationDataViewMapping> mappings;

    public ANotificationView GetView(NotificationData data)
    {
        if (!mappings.Any(m => m.ViewId == data.TemplateId))
        {
            return mappings.Single(m => m.ViewId == "generic_view").View;
        }

        return mappings.Single(m => m.ViewId == data.TemplateId).View;
    }

    [Serializable]
    public struct NotificationDataViewMapping
    {
        public string ViewId;
        public ANotificationView View;
    }
}
