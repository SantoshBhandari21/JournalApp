namespace JournalApp.Services;

public class ThemeService
{
    private const string Key = "app_theme";

    public string CurrentTheme { get; private set; } = "light";

    public event Action? OnChange;

    public void Load()
    {
        CurrentTheme = Preferences.Get(Key, "light");
        OnChange?.Invoke();
    }

    public void SetTheme(string theme)
    {
        if (theme != "light" && theme != "dark") theme = "light";

        CurrentTheme = theme;
        Preferences.Set(Key, CurrentTheme);
        OnChange?.Invoke();
    }

    public void Toggle()
    {
        SetTheme(CurrentTheme == "dark" ? "light" : "dark");
    }
}
