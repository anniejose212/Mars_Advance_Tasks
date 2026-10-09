// FILE: Config.cs
// ROLE: Strongly-typed model for testsettings.json.

namespace Task2.Config
{
    public class TestSettings
    {
        public BrowserSettings Browser { get; set; } = new BrowserSettings();
        public ReportSettings Report { get; set; } = new ReportSettings();
        public EnvironmentSettings Environment { get; set; } = new EnvironmentSettings();
        public LoginSettings Login { get; set; } = new LoginSettings();
        public LoginSettings SecondLogin { get; set; } = new LoginSettings();
    }

    public class BrowserSettings
    {
        public string Type { get; set; } = "";
        public bool Headless { get; set; }
        public int TimeoutSeconds { get; set; }
    }

    public class ReportSettings
    {
        public string Path { get; set; } = "";
        public string Title { get; set; } = "";
    }

    public class EnvironmentSettings
    {
        public string BaseUrl { get; set; } = "";
    }

    public class LoginSettings
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}