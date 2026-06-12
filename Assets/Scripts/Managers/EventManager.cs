using System;

public static class EventManager
{
    public static event Action OnLanguageChanged;

    public static void TriggerLanguageChanged()
    {
        OnLanguageChanged?.Invoke();
    }
}