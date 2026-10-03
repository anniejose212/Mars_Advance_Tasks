// FILE: ShareSkillsTests.cs
// ROLE: NUnit regression suite for Share Skill (/Home/ServiceListing).
//       Each test starts with no listings; a successful save is checked in Manage Listings.

using NUnit.Framework;
using Task1.Assertions;
using Task1.Hooks;
using Task1.Pages.Components.ShareSkills;
using Task1.Support;
using Task1.TestDataModel;

namespace Task1.Tests
{
    [TestFixture]
    public class ShareSkillsTests : Base
    {
        private ShareSkillModel _data;
        private ShareSkillsComponent _shareSkill;

        protected override void BeforeEachTest()
        {
            _data = JsonFileReader.Load<ShareSkillModel>(
                TestDataPath.Resolve("ShareSkillData.json"));
            _shareSkill = new ShareSkillsComponent(Driver);

            // Start every test with no listings
            Nav.NavigateTo("Home/ListingManagement");
            DataCleanupHooks.CleanListings(_shareSkill, "PRE-CLEAN");

            Nav.NavigateTo("Account/Profile");
            _shareSkill.OpenShareSkillsPage();
        }

        protected override void AfterEachTest()
        {
            Nav.NavigateTo("Home/ListingManagement");
            DataCleanupHooks.CleanListings(_shareSkill, "TEARDOWN");
        }

        // =====================================================================
        // POSITIVE
        // =====================================================================

        // SHS_TC_001 - Hourly, Online, Skill-exchange, Active
        [Test]
        [Category("Share_Skill")]
        [Category("Smoke")]
        public void SHS_TC_001_AddHourlySkillExchange_AppearsInListings()
        {
            var d = _data.ValidHourlySkillExchange;

            _shareSkill.FillListing(d);
            _shareSkill.Save();

            int count = _shareSkill.CountListingsWithTitle(d.Title);
            string details = _shareSkill.GetListingsDetails();

            ShareSkillsAssertions.AssertListingSaved(count, d.Title, details);
        }

        // SHS_TC_002 - One-off, On-site, Credit, Hidden
        [Test]
        [Category("Share_Skill")]
        public void SHS_TC_002_AddOneOffCredit_AppearsInListings()
        {
            var d = _data.ValidOneOffCredit;

            _shareSkill.FillListing(d);
            _shareSkill.Save();

            int count = _shareSkill.CountListingsWithTitle(d.Title);
            string details = _shareSkill.GetListingsDetails();

            ShareSkillsAssertions.AssertListingSaved(count, d.Title, details);
        }

        // SHS_TC_014 - Cancel leaves the form without saving
        [Test]
        [Category("Share_Skill")]
        public void SHS_TC_014_Cancel_ReturnsToProfile()
        {
            var d = _data.ValidOneOffCredit;

            _shareSkill.FillListing(d);
            _shareSkill.Cancel();

            ShareSkillsAssertions.AssertUrlContains(Driver.Url, "Profile");
        }

        // =====================================================================
        // NEGATIVE
        // =====================================================================

        // SHS_TC_003 - Save with nothing filled in
        [Test]
        [Category("Share_Skill")]
        public void SHS_TC_003_EmptyForm_ShowsError()
        {
            var d = _data.EmptyForm;

            _shareSkill.FillListing(d);
            _shareSkill.Save();

            string toastText = Toasts.GetToastText();

            ShareSkillsAssertions.AssertErrorToast(toastText, _data.ErrorToast);
        }

        // SHS_TC_004 - Title left empty
        [Test]
        [Category("Share_Skill")]
        public void SHS_TC_004_EmptyTitle_ShowsError()
        {
            var d = _data.EmptyTitle;

            _shareSkill.FillListing(d);
            _shareSkill.Save();

            string toastText = Toasts.GetToastText();

            ShareSkillsAssertions.AssertErrorToast(toastText, _data.ErrorToast);
        }

        // SHS_TC_005 - Description left empty
        [Test]
        [Category("Share_Skill")]
        public void SHS_TC_005_EmptyDescription_ShowsError()
        {
            var d = _data.EmptyDescription;

            _shareSkill.FillListing(d);
            _shareSkill.Save();

            string toastText = Toasts.GetToastText();

            ShareSkillsAssertions.AssertErrorToast(toastText, _data.ErrorToast);
        }

        // SHS_TC_006 - No tags added
        [Test]
        [Category("Share_Skill")]
        public void SHS_TC_006_NoTags_ShowsError()
        {
            var d = _data.NoTags;

            _shareSkill.FillListing(d);
            _shareSkill.Save();

            string toastText = Toasts.GetToastText();

            ShareSkillsAssertions.AssertErrorToast(toastText, _data.ErrorToast);
        }

        // SHS_TC_007 - Title with special characters only
        [Test]
        [Category("Share_Skill")]
        public void SHS_TC_007_SpecialCharsTitle_ShowsError()
        {
            var d = _data.SpecialCharsTitle;

            _shareSkill.FillListing(d);
            _shareSkill.Save();

            string toastText = Toasts.GetToastText();

            ShareSkillsAssertions.AssertErrorToast(toastText, _data.ErrorToast);
        }

        // SHS_TC_008 - Description with special characters only
        [Test]
        [Category("Share_Skill")]
        public void SHS_TC_008_SpecialCharsDescription_ShowsError()
        {
            var d = _data.SpecialCharsDescription;

            _shareSkill.FillListing(d);
            _shareSkill.Save();

            string toastText = Toasts.GetToastText();

            ShareSkillsAssertions.AssertErrorToast(toastText, _data.ErrorToast);
        }

        // =====================================================================
        // SECURITY
        // These fail if the app ever starts accepting unsafe input
        // =====================================================================

        // SHS_TC_009 - Script tag in Title
        [Test]
        [Category("Share_Skill")]
        [Category("Security")]
        public void SHS_TC_009_XssTitle_ShouldBeRejected()
        {
            var d = _data.XssTitle;

            _shareSkill.FillListing(d);
            _shareSkill.Save();

            string toastText = Toasts.GetToastText();

            ShareSkillsAssertions.AssertErrorToast(toastText, _data.ErrorToast);
        }

        // SHS_TC_010 - SQL injection in Description
        [Test]
        [Category("Share_Skill")]
        [Category("Security")]
        public void SHS_TC_010_SqlInjectionDescription_ShouldBeRejected()
        {
            var d = _data.SqlInjectionDescription;

            _shareSkill.FillListing(d);
            _shareSkill.Save();

            string toastText = Toasts.GetToastText();

            ShareSkillsAssertions.AssertErrorToast(toastText, _data.ErrorToast);
        }

        // =====================================================================
        // BOUNDARY
        // =====================================================================

        // SHS_TC_011 - Title stops at 100 characters
        [Test]
        [Category("Share_Skill")]
        public void SHS_TC_011_TitleOverMax_IsCappedAt100()
        {
            var d = _data.TitleOverMax;

            int length = _shareSkill.EnterAndGetLength("Title", d.Input);

            ShareSkillsAssertions.AssertFieldLength("Title", length, d.ExpectedLength);
        }

        // SHS_TC_012 - Description stops at 600 characters
        [Test]
        [Category("Share_Skill")]
        public void SHS_TC_012_DescriptionOverMax_IsCappedAt600()
        {
            var d = _data.DescriptionOverMax;

            int length = _shareSkill.EnterAndGetLength("Description", d.Input);

            ShareSkillsAssertions.AssertFieldLength("Description", length, d.ExpectedLength);
        }

        // SHS_TC_013 - Credit above 10 is blocked
        [Test]
        [Category("Share_Skill")]
        public void SHS_TC_013_CreditOverTen_IsBlocked()
        {
            var d = _data.CreditOverMax;

            int length = _shareSkill.EnterAndGetLength("Credit", d.Input);

            ShareSkillsAssertions.AssertFieldLength("Credit", length, d.ExpectedLength);
        }
    }
}