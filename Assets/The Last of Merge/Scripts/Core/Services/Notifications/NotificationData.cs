using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NotificationData", menuName = "Scriptable Objects/NotificationData")]
public class NotificationData : ScriptableObject
{
    public string Id; // Used for quick search for necessary notification in code
    public string TemplateId;
    public Sprite Sprite;
    public string TextKey; // If "none", "TranslationData" should be used
    public List<NotificationButtonData> ButtonData;
    public List<NotificationTranslationData> TranslationData;
}

[Serializable]
public struct NotificationButtonData
{
    public string ButtonKey; // Translation
    public string ActionId; // Mapping for action
}

/// <summary>
/// Fallback translations when primary <see cref="NotificationData.TextKey"/> is not available
/// </summary>
[Serializable]
public struct NotificationTranslationData
{
    public string LanguageCode;
    public string Translation;
}
