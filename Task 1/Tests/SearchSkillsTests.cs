// FILE: SearchSkillsTests.cs
// ROLE: NUnit regression suite for Search Skills (top bar search + results page).
//       Each test creates one listing first, then searches for it.

using NUnit.Framework;
using Task1.Assertions;
using Task1.Hooks;
using Task1.Pages.Components.SearchSkills;
using Task1.Pages.Components.ShareSkills;
using Task1.Support;
using Task1.TestDataModel;

namespace Task1.Tests
{
    [TestFixture]
    public class SearchSkillsTests : Base
    {
        private SearchSkillModel _data;
        private ShareSkillsComponent _shareSkill;
        private SearchSkillsComponent _search;

        protected override void BeforeEachTest()
        {
            _data = JsonFileReader.Load<SearchSkillModel>(
                TestDataPath.Resolve("SearchSkillData.json"));
            _shareSkill = new ShareSkillsComponent(Driver);
            _search = new SearchSkillsComponent(Driver);

            // Start with no listings, then create the one every test searches for
            Nav.NavigateTo("Home/ListingManagement");
            DataCleanupHooks.CleanListings(_shareSkill, "PRE-CLEAN");

            Nav.NavigateTo("Account/Profile");
            _shareSkill.OpenShareSkillsPage();
            _shareSkill.FillListing(_data.Listing);
            _shareSkill.Save();

            int count = _shareSkill.CountListingsWithTitle(_data.Listing.Title);
            string details = _shareSkill.GetListingsDetails();
            ShareSkillsAssertions.AssertListingSaved(count, _data.Listing.Title, details);
        }

        protected override void AfterEachTest()
        {
            Nav.NavigateTo("Home/ListingManagement");
            DataCleanupHooks.CleanListings(_shareSkill, "TEARDOWN");
        }

        // =====================================================================
        // SEARCH - POSITIVE
        // =====================================================================

        // SRS_TC_001 - Search by exact title (Enter key)
        [Test]
        [Category("Search_Skills")]
        [Category("Smoke")]
        public void SRS_TC_001_SearchExactTitle_ShowsListing()
        {
            string title = _data.Listing.Title;

            _search.SearchByEnter(title);

            int count = _search.CountResultsWithTitle(title);
            string details = _search.GetResultsDetails();

            SearchSkillsAssertions.AssertListingFound(count, title, details);
        }

        // SRS_TC_002 - Search by clicking the search icon
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_002_SearchByIcon_ShowsListing()
        {
            string title = _data.Listing.Title;

            _search.SearchByIcon(title);

            int count = _search.CountResultsWithTitle(title);
            string details = _search.GetResultsDetails();

            SearchSkillsAssertions.AssertListingFound(count, title, details);
        }

        // SRS_TC_003 - Search by part of the title
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_003_SearchPartialTitle_ShowsListing()
        {
            string title = _data.Listing.Title;

            _search.SearchByEnter(_data.PartialTitle);

            int count = _search.CountResultsWithTitle(title);
            string details = _search.GetResultsDetails();

            SearchSkillsAssertions.AssertListingFound(count, title, details);
        }

        // SRS_TC_004 - Search in lower case
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_004_SearchDifferentCase_ShowsListing()
        {
            string title = _data.Listing.Title;

            _search.SearchByEnter(_data.DifferentCaseTitle);

            int count = _search.CountResultsWithTitle(title);
            string details = _search.GetResultsDetails();

            SearchSkillsAssertions.AssertListingFound(count, title, details);
        }

        // SRS_TC_018 - Spaces around the title are ignored
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_018_SearchTrimmedTitle_ShowsListing()
        {
            string title = _data.Listing.Title;

            _search.SearchByEnter(_data.TrimmedTitle);

            int count = _search.CountResultsWithTitle(title);
            string details = _search.GetResultsDetails();

            SearchSkillsAssertions.AssertListingFound(count, title, details);
        }

        // SRS_TC_014 - Search by user shows that user's listing.
        // Searches the title first because "Search user" only exists on the results page.
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_014_SearchUser_ShowsListing()
        {
            string title = _data.Listing.Title;

            _search.SearchByEnter(title);
            _search.SearchUser(_data.SellerName);

            int count = _search.CountResultsWithTitle(title);
            string details = _search.GetResultsDetails();

            SearchSkillsAssertions.AssertListingFound(count, title, details);
        }

        // SRS_TC_017 - Clicking a result opens the listing detail page
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_017_OpenResult_ShowsServiceDetail()
        {
            _search.SearchByEnter(_data.Listing.Title);
            _search.OpenListing(_data.Listing.Title);

            SearchSkillsAssertions.AssertUrlContains(Driver.Url, "ServiceDetail");
        }

        // =====================================================================
        // CATEGORIES AND FILTERS - POSITIVE
        // =====================================================================

        // SRS_TC_005 - Category count shows the one listing
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_005_CategoryCount_ShowsOne()
        {
            _search.SearchByEnter(_data.Listing.Title);
            _search.CountResultsWithTitle(_data.Listing.Title); // only waits for the results to load

            string actual = _search.GetCategoryCount(_data.Category);

            SearchSkillsAssertions.AssertCategoryCount(actual, _data.ExpectedCategoryCount, _data.Category);
        }

        // SRS_TC_006 - Category filter keeps the listing
        [Test]
        [Category("Search_Skills")]
        [Category("Smoke")]
        public void SRS_TC_006_FilterCategory_ShowsListing()
        {
            string title = _data.Listing.Title;

            _search.SearchByEnter(title);
            _search.SelectCategory(_data.Category);

            int count = _search.CountResultsWithTitle(title);
            string details = _search.GetResultsDetails();

            SearchSkillsAssertions.AssertListingFound(count, title, details);
        }

        // SRS_TC_007 - Sub-category filter keeps the listing
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_007_FilterSubCategory_ShowsListing()
        {
            string title = _data.Listing.Title;

            _search.SearchByEnter(title);
            _search.SelectCategory(_data.Category);
            _search.SelectSubCategory(_data.SubCategory);

            int count = _search.CountResultsWithTitle(title);
            string details = _search.GetResultsDetails();

            SearchSkillsAssertions.AssertListingFound(count, title, details);
        }

        // SRS_TC_015 - All Categories shows the listing again
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_015_AllCategories_ShowsListing()
        {
            string title = _data.Listing.Title;

            _search.SearchByEnter(title);
            _search.SelectCategory(_data.Category);
            _search.SelectAllCategories();

            int count = _search.CountResultsWithTitle(title);
            string details = _search.GetResultsDetails();

            SearchSkillsAssertions.AssertListingFound(count, title, details);
        }

        // SRS_TC_008 - Online filter keeps an Online listing
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_008_FilterOnline_ShowsListing()
        {
            string title = _data.Listing.Title;

            _search.SearchByEnter(title);
            _search.SelectLocationFilter(_data.OnlineFilter);

            int count = _search.CountResultsWithTitle(title);
            string details = _search.GetResultsDetails();

            SearchSkillsAssertions.AssertListingFound(count, title, details);
        }

        // SRS_TC_016 - ShowAll brings the listing back after Onsite hid it
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_016_FilterShowAll_ShowsListing()
        {
            string title = _data.Listing.Title;

            _search.SearchByEnter(title);
            _search.SelectLocationFilter(_data.OnsiteFilter);
            _search.SelectLocationFilter(_data.ShowAllFilter);

            int count = _search.CountResultsWithTitle(title);
            string details = _search.GetResultsDetails();

            SearchSkillsAssertions.AssertListingFound(count, title, details);
        }

        // =====================================================================
        // NEGATIVE
        // =====================================================================

        // SRS_TC_009 - Text that matches nothing
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_009_SearchNoMatch_ShowsNoResults()
        {
            _search.SearchByEnter(_data.NoMatchText);

            string message = _search.GetNoResultsMessage();

            SearchSkillsAssertions.AssertNoResults(message, _data.NoResultsText);
        }

        // SRS_TC_019 - Special characters only return nothing
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_019_SearchSpecialChars_ShowsNoResults()
        {
            _search.SearchByEnter(_data.SpecialCharsText);

            string message = _search.GetNoResultsMessage();

            SearchSkillsAssertions.AssertNoResults(message, _data.NoResultsText);
        }

        // SRS_TC_010 - A different category hides the listing
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_010_FilterOtherCategory_ShowsNoResults()
        {
            _search.SearchByEnter(_data.Listing.Title);
            _search.SelectCategory(_data.OtherCategory);

            string message = _search.GetNoResultsMessage();

            SearchSkillsAssertions.AssertNoResults(message, _data.NoResultsText);
        }

        // SRS_TC_011 - Onsite filter hides an Online listing
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_011_FilterOnsite_ShowsNoResults()
        {
            _search.SearchByEnter(_data.Listing.Title);
            _search.SelectLocationFilter(_data.OnsiteFilter);

            string message = _search.GetNoResultsMessage();

            SearchSkillsAssertions.AssertNoResults(message, _data.NoResultsText);
        }

        // SRS_TC_012 - A Hidden listing does not appear in search
        [Test]
        [Category("Search_Skills")]
        public void SRS_TC_012_HiddenListing_ShowsNoResults()
        {
            Nav.NavigateTo("Account/Profile");
            _shareSkill.OpenShareSkillsPage();
            _shareSkill.FillListing(_data.HiddenListing);
            _shareSkill.Save();
            _shareSkill.CountListingsWithTitle(_data.HiddenListing.Title); // only waits for the save to finish

            _search.SearchByEnter(_data.HiddenListing.Title);

            string message = _search.GetNoResultsMessage();

            SearchSkillsAssertions.AssertNoResults(message, _data.NoResultsText);
        }

        // =====================================================================
        // SECURITY AND DESTRUCTIVE
        // =====================================================================

        // SRS_TC_013 - Script tag in the search box returns nothing and runs nothing
        [Test]
        [Category("Search_Skills")]
        [Category("Security")]
        public void SRS_TC_013_UnsafeSearchText_ShowsNoResults()
        {
            _search.SearchByEnter(_data.UnsafeText);

            string message = _search.GetNoResultsMessage();

            SearchSkillsAssertions.AssertNoResults(message, _data.NoResultsText);
        }

        // SRS_TC_020 - A huge search text is handled without breaking the page
        [Test]
        [Category("Search_Skills")]
        [Category("Destructive")]
        public void SRS_TC_020_SearchHugePayload_ShowsNoResults()
        {
            string payload = new string('A', _data.HugePayloadLength);

            _search.SearchByEnter(payload);

            string message = _search.GetNoResultsMessage();

            SearchSkillsAssertions.AssertNoResults(message, _data.NoResultsText);
        }
    }
}