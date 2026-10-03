// FILE: SearchSkillsAssertions.cs
// ROLE: Assertions for the Search Skills suite. Logs actual vs expected to TestContext.

using NUnit.Framework;

namespace Task1.Assertions
{
    public static class SearchSkillsAssertions
    {
        public static void AssertListingFound(int count, string title, string details)
        {
            TestContext.WriteLine($"Listing -> '{title}' | Found: {count}");
            TestContext.WriteLine($"[RESULTS] {details}");
            Assert.That(count, Is.EqualTo(1),
                $"Expected '{title}' once in search results but found {count}. Results: [{details}]");
        }

        public static void AssertNoResults(string message, string expected)
        {
            TestContext.WriteLine($"Message: {message}");
            Assert.That(message, Does.Contain(expected).IgnoreCase,
                $"Expected '{expected}' but was '{message}'.");
        }

        public static void AssertCategoryCount(string actual, string expected, string category)
        {
            TestContext.WriteLine($"Category -> '{category}' | Expected: {expected} | Actual: {actual}");
            Assert.That(actual, Is.EqualTo(expected),
                $"Expected '{category}' count {expected} but was {actual}.");
        }

        public static void AssertUrlContains(string url, string expectedPart)
        {
            TestContext.WriteLine($"URL: {url}");
            Assert.That(url, Does.Contain(expectedPart).IgnoreCase,
                $"Expected URL to contain '{expectedPart}' but was '{url}'.");
        }
    }
}
