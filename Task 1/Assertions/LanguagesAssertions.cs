using NUnit.Framework;

namespace Task1.Assertions
{
    public static class LanguagesAssertions
    {
        private const string SecurityPolicy =
            "POLICY EXPECTATION: Unsafe input must be rejected server-side; no record created or displayed. " +
            "Enforce strict allowlist validation and HTML-encode on render.";

        public static void AssertSuccessToast(string toast, string expectedFragment)
        {
            TestContext.WriteLine($"Toast: {toast}");
            Assert.That(toast, Is.Not.Null.And.Not.Empty, "Expected a success toast.");
            Assert.That(toast, Does.Contain(expectedFragment).IgnoreCase,
                $"Expected toast to contain '{expectedFragment}' but was '{toast}'.");
        }

        public static void AssertErrorToast(string toast, string expectedFragment)
        {
            TestContext.WriteLine($"Toast: {toast}");
            Assert.That(toast, Is.Not.Null.And.Not.Empty, "Expected an error toast.");
            Assert.That(toast, Does.Contain(expectedFragment).IgnoreCase,
                $"Expected toast to contain '{expectedFragment}' but was '{toast}'.");
        }

        public static void AssertLanguageVisible(int count, string language, string level, string details)
        {
            TestContext.WriteLine($"Language -> '{language}' / '{level}' | Found: {count}");
            if (count == 0)
                Assert.Fail($"Missing (\"{language}\", \"{level}\"). Table: [{details}]");
        }

        public static void AssertLanguageNotVisible(int count, string language, string level, string details)
        {
            TestContext.WriteLine($"Language -> '{language}' / '{level}' | Found: {count}");
            if (count > 0)
                Assert.Fail($"Found (\"{language}\", \"{level}\") {count} time(s). Table: [{details}]");
        }

        public static void AssertLanguageOccurrences(int actual, int expected, string language, string level, string details)
        {
            TestContext.WriteLine($"Language -> '{language}' / '{level}' | Expected: {expected} | Found: {actual}");
            Assert.That(actual, Is.EqualTo(expected),
                $"Expected {expected} occurrence(s) of (\"{language}\", \"{level}\") but found {actual}. Table: [{details}]");
        }

        public static void AssertLanguagesCount(int actual, int expected, string details)
        {
            TestContext.WriteLine($"Languages in list: [{details}]");
            Assert.That(actual, Is.EqualTo(expected),
                $"Expected {expected} language(s) but found {actual}. Actual list: [{details}]");
        }

        public static void AssertAddNewButtonHidden(bool isDisplayed, int count)
        {
            TestContext.WriteLine($"Add New button displayed: {isDisplayed} | Languages in list: {count}");
            Assert.That(isDisplayed, Is.False,
                $"Expected 'Add New' button to be hidden, but it was visible. Found {count} languages.");
        }

        public static void AssertUnsafeInputRejected(string alertText, int totalRows, string payload, string details)
        {
            TestContext.WriteLine($"Payload: {payload}");
            TestContext.WriteLine(alertText == null ? "[NO ALERT DETECTED]" : $"[ALERT DETECTED] '{alertText}'");
            TestContext.WriteLine($"[FULL TABLE] {details}");

            Assert.That(alertText, Is.Null,
                $"{SecurityPolicy}\nViolation: script executed (XSS). Alert text: '{alertText}'.");

            Assert.That(totalRows, Is.EqualTo(0),
                $"{SecurityPolicy}\nViolation: unsafe input was persisted. Table: [{details}]");
        }
        

        public static void AssertInvalidLevelRejected(bool levelExisted, string level, string toast, int total, string details)
        {
            TestContext.WriteLine(levelExisted
                ? $"Note: level '{level}' existed in the dropdown. Ensure server-side allowlist."
                : $"Level '{level}' did not exist in the dropdown, as expected.");

            AssertErrorToast(toast, "Please enter");
            AssertLanguagesCount(total, 0, details);
        }

        public static void AssertMassiveNameHandled(string toast, int savedCount, int length)
        {
            TestContext.WriteLine($"Toast: {toast}");
            TestContext.WriteLine($"Massive language saved: {savedCount > 0}");
            Assert.That(toast, Is.Not.Null.And.Not.Empty,
                $"App gave no response to a {length}-character language name.");
        }
    }
}