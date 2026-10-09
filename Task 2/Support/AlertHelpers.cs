// FILE: AlertHelpers.cs
// ROLE: Extension methods for safely handling browser alerts during teardown.

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Task2.Support
{
    public static class AlertHelpers
    {
        // Returns the open alert, or null if none appears within the given seconds
        public static IAlert? TryGetAlert(this IWebDriver driver, int seconds = 2)
        {
            if (driver == null) return null;

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
            try
            {
                return wait.Until<IAlert?>(d =>
                {
                    try { return d.SwitchTo().Alert(); }
                    catch (NoAlertPresentException) { return null; }
                });
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        // Accepts a leftover alert so it cannot block the next test. Returns true if one was closed.
        public static bool TryDismissAnyAlert(this IWebDriver driver, int seconds = 0)
        {
            if (driver == null) return false;

            var alert = driver.TryGetAlert(seconds);
            if (alert == null) return false;

            try
            {
                Console.WriteLine($"[TEARDOWN] Dismissing stray alert: '{alert.Text}'");
                alert.Accept();
                return true;
            }
            catch (WebDriverException)
            {
                // The alert closed on its own before Accept; nothing left to do
                return false;
            }
        }
    }
}
