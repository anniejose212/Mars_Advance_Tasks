// FILE: AboutMeTests.cs
// ROLE: NUnit regression suite for the Profile "About Me" panel.

using NUnit.Framework;
using Task1.Assertions;
using Task1.Hooks;
using Task1.Support;
using Task1.TestDataModel;

namespace Task1.Tests
{
    [TestFixture]
    public class AboutMeTests : Base
    {
        private AboutMeModel _data;

        protected override void BeforeEachTest()
        {
            _data = JsonFileReader.Load<AboutMeModel>(
                TestDataPath.Resolve("AboutMeData.json"));

            Nav.NavigateTo("Account/Profile");
            DataCleanupHooks.ResetProfileToDefaults(AboutMePage, Toasts, _data.Defaults);
        }

        // PRF_TC_005 changes the name. Other suites search by seller name, so put it back.
        protected override void AfterEachTest()
        {
            Nav.NavigateTo("Account/Profile");
            DataCleanupHooks.ResetNameToDefault(AboutMePage, _data.Defaults);
        }

        // =====================================================================
        // PRF-TC-001 - Update all three fields together
        // =====================================================================
        [Test]
        [Category("Profile_Settings")]
        [Category("Smoke")]
        public void PRF_TC_001_UpdateAllFields_AllValuesPersist()
        {
            string beforeAvailability = AboutMePage.GetSelectedAvailability();
            string beforeHours = AboutMePage.GetSelectedHours();
            string beforeEarnTarget = AboutMePage.GetSelectedEarnTarget();

            // Each save shows a toast; wait for it and close it before the next edit
            AboutMePage.SelectAvailability(_data.Availability);
            Toasts.GetToastText();
            Toasts.CloseToastAndWait();

            AboutMePage.SelectHours(_data.Hours);
            Toasts.GetToastText();
            Toasts.CloseToastAndWait();

            AboutMePage.SelectEarnTarget(_data.EarnTarget);
            string toastText = Toasts.GetToastText();

            string savedAvailability = AboutMePage.GetSelectedAvailability();
            string savedHours = AboutMePage.GetSelectedHours();
            string savedEarnTarget = AboutMePage.GetSelectedEarnTarget();

            ProfileAssertions.AssertSuccessToast(toastText, _data.SuccessToast);

            ProfileAssertions.AssertAllFieldsUpdated(
                beforeAvailability, savedAvailability, _data.Availability,
                beforeHours, savedHours, _data.Hours,
                beforeEarnTarget, savedEarnTarget, _data.EarnTarget);
        }

        // =====================================================================
        // PRF-TC-002 - Update Availability only
        // =====================================================================
        [Test]
        [Category("Profile_Settings")]
        public void PRF_TC_002_UpdateAvailabilityOnly_OtherFieldsUnchanged()
        {
            string beforeAvailability = AboutMePage.GetSelectedAvailability();
            string originalHours = AboutMePage.GetSelectedHours();
            string originalEarnTarget = AboutMePage.GetSelectedEarnTarget();

            AboutMePage.SelectAvailability(_data.Availability);
            string toastText = Toasts.GetToastText();

            string savedAvailability = AboutMePage.GetSelectedAvailability();
            string savedHours = AboutMePage.GetSelectedHours();
            string savedEarnTarget = AboutMePage.GetSelectedEarnTarget();

            ProfileAssertions.AssertAvailabilityUpdatedOnly(
                toastText,
                beforeAvailability, savedAvailability, _data.Availability,
                savedHours, originalHours,
                savedEarnTarget, originalEarnTarget);
        }

        // =====================================================================
        // PRF-TC-003 - Update Hours only
        // =====================================================================
        [Test]
        [Category("Profile_Settings")]
        public void PRF_TC_003_UpdateHoursOnly_OtherFieldsUnchanged()
        {
            string beforeHours = AboutMePage.GetSelectedHours();
            string originalAvailability = AboutMePage.GetSelectedAvailability();
            string originalEarnTarget = AboutMePage.GetSelectedEarnTarget();

            AboutMePage.SelectHours(_data.Hours);
            string toastText = Toasts.GetToastText();

            string savedHours = AboutMePage.GetSelectedHours();
            string savedAvailability = AboutMePage.GetSelectedAvailability();
            string savedEarnTarget = AboutMePage.GetSelectedEarnTarget();

            ProfileAssertions.AssertHoursUpdatedOnly(
                toastText,
                beforeHours, savedHours, _data.Hours,
                savedAvailability, originalAvailability,
                savedEarnTarget, originalEarnTarget);
        }

        // =====================================================================
        // PRF-TC-004 - Update Earn Target only
        // =====================================================================
        [Test]
        [Category("Profile_Settings")]
        public void PRF_TC_004_UpdateEarnTargetOnly_OtherFieldsUnchanged()
        {
            string beforeEarnTarget = AboutMePage.GetSelectedEarnTarget();
            string originalAvailability = AboutMePage.GetSelectedAvailability();
            string originalHours = AboutMePage.GetSelectedHours();

            AboutMePage.SelectEarnTarget(_data.EarnTarget);
            string toastText = Toasts.GetToastText();

            string savedEarnTarget = AboutMePage.GetSelectedEarnTarget();
            string savedAvailability = AboutMePage.GetSelectedAvailability();
            string savedHours = AboutMePage.GetSelectedHours();

            ProfileAssertions.AssertEarnTargetUpdatedOnly(
                toastText,
                beforeEarnTarget, savedEarnTarget, _data.EarnTarget,
                savedAvailability, originalAvailability,
                savedHours, originalHours);
        }

        // =====================================================================
        // PRF-TC-005 - Update first and last name
        // =====================================================================
        [Test]
        [Category("Profile_Settings")]
        public void PRF_TC_005_UpdateName_ShowsNewName()
        {
            AboutMePage.UpdateName(_data.NewFirstName, _data.NewLastName);
            Nav.NavigateTo("Account/Profile");

            string displayed = AboutMePage.GetDisplayedName();

            ProfileAssertions.AssertDisplayedName(displayed, _data.NewFirstName + " " + _data.NewLastName);
        }

        // =====================================================================
        // PRF-TC-006 - Location is read-only
        // =====================================================================
        [Test]
        [Category("Profile_Settings")]
        public void PRF_TC_006_LocationIsReadOnly()
        {
            bool isVisible = AboutMePage.IsLocationVisible();
            bool isEditable = AboutMePage.IsLocationEditable();

            ProfileAssertions.AssertLocationReadOnly(isVisible, isEditable);
        }
    }
}