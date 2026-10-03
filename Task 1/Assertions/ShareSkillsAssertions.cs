// FILE: ShareSkillsAssertions.cs
// ROLE: Assertions for the Share Skill suite. Logs actual vs expected to TestContext.

using NUnit.Framework;

namespace Task1.Assertions
{
    public static class ShareSkillsAssertions
    {
        // A saved listing appears exactly once in Manage Listings
        public static void AssertListingSaved(int count, string title, string details)
        {
            TestContext.WriteLine($"Listing -> '{title}' | Found: {count}");
            TestContext.WriteLine($"[MANAGE LISTINGS] {details}");
            Assert.That(count, Is.EqualTo(1),
                $"Expected listing '{title}' once in Manage Listings but found {count}. Table: [{details}]");
        }

        // An invalid form shows an error toast (read with ToastHelper.GetErrorToastText)
        public static void AssertErrorToast(string toast, string expectedFragment)
        {
            TestContext.WriteLine($"Toast: {toast}");
            Assert.That(toast, Does.Contain(expectedFragment).IgnoreCase,
                $"Expected toast to contain '{expectedFragment}' but was '{toast}'.");
        }

        // A field's limit stops extra characters
        public static void AssertFieldLength(string field, int actualLength, int expectedLength)
        {
            TestContext.WriteLine($"Field: {field} | Expected: {expectedLength} | Actual: {actualLength}");
            Assert.That(actualLength, Is.EqualTo(expectedLength),
                $"Expected '{field}' to hold {expectedLength} character(s) but it held {actualLength}.");
        }

        public static void AssertUrlContains(string url, string expectedPart)
        {
            TestContext.WriteLine($"URL: {url}");
            Assert.That(url, Does.Contain(expectedPart).IgnoreCase,
                $"Expected URL to contain '{expectedPart}' but was '{url}'.");
        }
    }
}
