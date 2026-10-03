// FILE: NotificationAssertions.cs
// ROLE: Assertions for the Notifications suite. Logs actual vs expected to TestContext.

using NUnit.Framework;

namespace Task1.Assertions
{
    public static class NotificationAssertions
    {
        public static void AssertRequestSentToast(string toast, string expected)
        {
            TestContext.WriteLine($"Toast: {toast}");
            Assert.That(toast, Does.Contain(expected).IgnoreCase,
                $"Expected '{expected}' toast but was '{toast}'.");
        }

        public static void AssertUnreadCount(string actual, string expected)
        {
            TestContext.WriteLine($"Unread badge -> Expected: {expected} | Actual: {actual}");
            Assert.That(actual, Is.EqualTo(expected),
                $"Expected unread count {expected} but was {actual}.");
        }

        public static void AssertNotificationShown(string messages, string expectedText)
        {
            TestContext.WriteLine($"[NOTIFICATIONS] {messages}");
            Assert.That(messages, Does.Contain(expectedText).IgnoreCase,
                $"Expected a notification containing '{expectedText}'. Found: [{messages}]");
        }

        public static void AssertCount(int actual, int expected, string what)
        {
            TestContext.WriteLine($"{what} -> Expected: {expected} | Actual: {actual}");
            Assert.That(actual, Is.EqualTo(expected),
                $"Expected {what} to be {expected} but was {actual}.");
        }

        public static void AssertMoreThan(int actual, int minimum, string what)
        {
            TestContext.WriteLine($"{what} -> Expected more than: {minimum} | Actual: {actual}");
            Assert.That(actual, Is.GreaterThan(minimum),
                $"Expected {what} to be more than {minimum} but was {actual}.");
        }

        public static void AssertButtonHidden(bool shown, string button)
        {
            TestContext.WriteLine($"Button '{button}' shown: {shown}");
            Assert.That(shown, Is.False,
                $"Expected '{button}' to be hidden when nothing is selected.");
        }

        public static void AssertUrlContains(string url, string expectedPart)
        {
            TestContext.WriteLine($"URL: {url}");
            Assert.That(url, Does.Contain(expectedPart).IgnoreCase,
                $"Expected URL to contain '{expectedPart}' but was '{url}'.");
        }
    }
}
