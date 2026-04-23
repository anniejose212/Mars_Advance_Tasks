// FILE: AlertHelpers.cs
// ROLE: Extension methods for safely handling browser alerts during teardown.

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Task1.Support
{
    public static class AlertHelpers
    {
        public static IAlert TryGetAlert(this IWebDriver driver, int seconds = 2)
        {
            if (driver == null) return null;

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
            try
            {
                return wait.Until(d =>
                {
                    try   { return d.SwitchTo().Alert(); }
                    catch (NoAlertPresentException) { return null; }
                });
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        public static bool TryDismissAnyAlert(this IWebDriver driver, int seconds = 0)
        {
            if (driver == null) return false;

            var alert = driver.TryGetAlert(seconds > 0 ? seconds : 0);
            if (alert == null) return false;

            try
            {
                Console.WriteLine($"[TEARDOWN] Dismissing stray alert: '{alert.Text}'");
                alert.Accept();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
