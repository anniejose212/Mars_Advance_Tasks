using NUnit.Framework;

namespace Task1.Assertions
{
    public static class ProfileAssertions
    {

        public static void AssertDisplayedName(string actual, string expected)
        {
            TestContext.WriteLine($"Name -> Expected: '{expected}' | Actual: '{actual}'");
            Assert.That(actual, Is.EqualTo(expected),
                $"Displayed name is wrong. Expected: '{expected}', Actual: '{actual}'");
        }
        public static void AssertSuccessToast(string actualMessage, string expectedMessage)
        {
            TestContext.WriteLine($"Toast: {actualMessage}");
            Assert.That(actualMessage, Is.EqualTo(expectedMessage),
                $"Success toast message is incorrect. Expected: '{expectedMessage}', Actual: '{actualMessage}'");
        }

        public static void AssertLocationReadOnly(bool isVisible, bool isEditable)
        {
            TestContext.WriteLine($"Location -> Visible: {isVisible} | Editable: {isEditable}");
            Assert.That(isVisible, Is.True, "Location field should be visible on the profile page");
            Assert.That(isEditable, Is.False, "Location field should be read-only (BUG-PRF-002)");
        }

        public static void AssertAllFieldsUpdated(
            string beforeAvailability, string savedAvailability, string expectedAvailability,
            string beforeHours, string savedHours, string expectedHours,
            string beforeEarnTarget, string savedEarnTarget, string expectedEarnTarget)
        {
            TestContext.WriteLine($"Availability -> Before: '{beforeAvailability}' | Expected: '{expectedAvailability}' | Actual: '{savedAvailability}'");
            TestContext.WriteLine($"Hours -> Before: '{beforeHours}' | Expected: '{expectedHours}' | Actual: '{savedHours}'");
            TestContext.WriteLine($"EarnTarget -> Before: '{beforeEarnTarget}' | Expected: '{expectedEarnTarget}' | Actual: '{savedEarnTarget}'");

            Assert.That(savedAvailability, Is.EqualTo(expectedAvailability),
                $"Availability did not persist. Expected: '{expectedAvailability}', Actual: '{savedAvailability}'");

            Assert.That(savedHours, Is.EqualTo(expectedHours),
                $"Hours did not persist. Expected: '{expectedHours}', Actual: '{savedHours}'");

            Assert.That(savedEarnTarget, Is.EqualTo(expectedEarnTarget),
                $"Earn Target did not persist. Expected: '{expectedEarnTarget}', Actual: '{savedEarnTarget}'");
        }

        public static void AssertAvailabilityUpdatedOnly(
            string toast,
            string beforeAvailability, string savedAvailability, string expectedAvailability,
            string savedHours, string originalHours,
            string savedEarnTarget, string originalEarnTarget)
        {
            TestContext.WriteLine($"Toast: {toast}");
            TestContext.WriteLine($"Availability -> Before: '{beforeAvailability}' | Expected: '{expectedAvailability}' | Actual: '{savedAvailability}'");
            TestContext.WriteLine($"Hours -> Original: '{originalHours}' | Actual: '{savedHours}'");
            TestContext.WriteLine($"EarnTarget -> Original: '{originalEarnTarget}' | Actual: '{savedEarnTarget}'");

            Assert.That(savedAvailability, Is.EqualTo(expectedAvailability),
                $"Availability did not persist. Expected: '{expectedAvailability}', Actual: '{savedAvailability}'");

            Assert.That(savedHours, Is.EqualTo(originalHours),
                $"Hours changed unexpectedly. Expected unchanged: '{originalHours}', Actual: '{savedHours}'");

            Assert.That(savedEarnTarget, Is.EqualTo(originalEarnTarget),
                $"Earn Target changed unexpectedly. Expected unchanged: '{originalEarnTarget}', Actual: '{savedEarnTarget}'");
        }

        public static void AssertHoursUpdatedOnly(
            string toast,
            string beforeHours, string savedHours, string expectedHours,
            string savedAvailability, string originalAvailability,
            string savedEarnTarget, string originalEarnTarget)
        {
            TestContext.WriteLine($"Toast: {toast}");
            TestContext.WriteLine($"Hours -> Before: '{beforeHours}' | Expected: '{expectedHours}' | Actual: '{savedHours}'");
            TestContext.WriteLine($"Availability -> Original: '{originalAvailability}' | Actual: '{savedAvailability}'");
            TestContext.WriteLine($"EarnTarget -> Original: '{originalEarnTarget}' | Actual: '{savedEarnTarget}'");

            Assert.That(savedHours, Is.EqualTo(expectedHours),
                $"Hours did not persist. Expected: '{expectedHours}', Actual: '{savedHours}'");

            Assert.That(savedAvailability, Is.EqualTo(originalAvailability),
                $"Availability changed unexpectedly. Expected unchanged: '{originalAvailability}', Actual: '{savedAvailability}'");

            Assert.That(savedEarnTarget, Is.EqualTo(originalEarnTarget),
                $"Earn Target changed unexpectedly. Expected unchanged: '{originalEarnTarget}', Actual: '{savedEarnTarget}'");
        }

        public static void AssertEarnTargetUpdatedOnly(
            string toast,
            string beforeEarnTarget, string savedEarnTarget, string expectedEarnTarget,
            string savedAvailability, string originalAvailability,
            string savedHours, string originalHours)
        {
            TestContext.WriteLine($"Toast: {toast}");
            TestContext.WriteLine($"EarnTarget -> Before: '{beforeEarnTarget}' | Expected: '{expectedEarnTarget}' | Actual: '{savedEarnTarget}'");
            TestContext.WriteLine($"Availability -> Original: '{originalAvailability}' | Actual: '{savedAvailability}'");
            TestContext.WriteLine($"Hours -> Original: '{originalHours}' | Actual: '{savedHours}'");

            Assert.That(savedEarnTarget, Is.EqualTo(expectedEarnTarget),
                $"Earn Target did not persist. Expected: '{expectedEarnTarget}', Actual: '{savedEarnTarget}'");

            Assert.That(savedAvailability, Is.EqualTo(originalAvailability),
                $"Availability changed unexpectedly. Expected unchanged: '{originalAvailability}', Actual: '{savedAvailability}'");

            Assert.That(savedHours, Is.EqualTo(originalHours),
                $"Hours changed unexpectedly. Expected unchanged: '{originalHours}', Actual: '{savedHours}'");
        }
    }
}