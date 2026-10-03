using NUnit.Framework;

namespace Task1.Assertions
{
    public static class SkillsAssertions
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

        public static void AssertSkillVisible(int count, string skill, string level, string details)
        {
            TestContext.WriteLine($"Skill -> '{skill}' / '{level}' | Found: {count}");
            if (count == 0)
                Assert.Fail($"Missing (\"{skill}\", \"{level}\"). Table: [{details}]");
        }

        public static void AssertSkillNotVisible(int count, string skill, string level, string details)
        {
            TestContext.WriteLine($"Skill -> '{skill}' / '{level}' | Found: {count}");
            if (count > 0)
                Assert.Fail($"Found (\"{skill}\", \"{level}\") {count} time(s). Table: [{details}]");
        }

        public static void AssertSkillOccurrences(int actual, int expected, string skill, string level, string details)
        {
            TestContext.WriteLine($"Skill -> '{skill}' / '{level}' | Expected: {expected} | Found: {actual}");
            Assert.That(actual, Is.EqualTo(expected),
                $"Expected {expected} occurrence(s) of (\"{skill}\", \"{level}\") but found {actual}. Table: [{details}]");
        }

        public static void AssertSkillsCount(int actual, int expected, string details)
        {
            TestContext.WriteLine($"Skills in list: [{details}]");
            Assert.That(actual, Is.EqualTo(expected),
                $"Expected {expected} skill(s) but found {actual}. Actual list: [{details}]");
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

        public static void AssertMassiveNameHandled(string toast, int savedCount, int length)
        {
            TestContext.WriteLine($"Toast: {toast}");
            TestContext.WriteLine($"Massive skill saved: {savedCount > 0}");
            Assert.That(toast, Is.Not.Null.And.Not.Empty,
                $"App gave no response to a {length}-character skill name.");
        }
    }
}