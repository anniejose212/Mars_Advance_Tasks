// FILE: DriverFactory.cs
// ROLE: Creates the WebDriver in one place, from the browser settings in testsettings.json.

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Task2.Support
{
    public static class DriverFactory
    {
        // browserType: "chrome" (default) or "firefox"
        public static IWebDriver Create(string browserType, bool headless)
        {
            string type = browserType?.ToLowerInvariant() ?? "chrome";

            if (type == "firefox")
            {
                var firefoxOptions = new FirefoxOptions();
                if (headless) firefoxOptions.AddArgument("--headless");
                return new FirefoxDriver(firefoxOptions);
            }

            var chromeOptions = new ChromeOptions();
            if (headless) chromeOptions.AddArgument("--headless=new");
            return new ChromeDriver(chromeOptions);
        }
    }
}
