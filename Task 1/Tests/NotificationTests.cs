// FILE: NotificationTests.cs
// ROLE: NUnit regression suite for Notifications.
//       Two users: A (Settings.Login) owns the listing and receives notifications,
//       B (Settings.SecondLogin) sends the trade request that creates them.
//       Grouped by component: Requests, Notification dropdown, Dashboard.

using NUnit.Framework;
using Task1.Assertions;
using Task1.Hooks;
using Task1.Pages.Components.Notifications;
using Task1.Pages.Components.SearchSkills;
using Task1.Pages.Components.ShareSkills;
using Task1.Support;
using Task1.TestDataModel;

namespace Task1.Tests
{
    [TestFixture]
    public class NotificationTests : Base
    {
        private NotificationModel _data;
        private ShareSkillsComponent _shareSkill;
        private SearchSkillsComponent _search;
        private ManageRequestsComponent _request;
        private NotificationDropdownComponent _dropdown;
        private DashboardComponent _dashboard;

        // Base has already logged in as A
        protected override void BeforeEachTest()
        {
            _data = JsonFileReader.Load<NotificationModel>(
                TestDataPath.Resolve("NotificationData.json"));
            _shareSkill = new ShareSkillsComponent(Driver);
            _search = new SearchSkillsComponent(Driver);
            _request = new ManageRequestsComponent(Driver);
            _dropdown = new NotificationDropdownComponent(Driver);
            _dashboard = new DashboardComponent(Driver);

            // B withdraws leftover requests first, otherwise A can't delete its listings
            LoginPage.LoginAs(Settings.SecondLogin.Username, Settings.SecondLogin.Password);
            Nav.NavigateTo("Account/Profile");
            DataCleanupHooks.CleanSentRequests(_request);

            // A starts with no listings and no unread notifications
            LoginPage.LoginAs(Settings.Login.Username, Settings.Login.Password);
            Nav.NavigateTo("Home/ListingManagement");
            DataCleanupHooks.CleanListings(_shareSkill, "PRE-CLEAN");
            DataCleanupHooks.CleanNotifications(_dropdown, "PRE-CLEAN");

            // A creates the listing B will request
            Nav.NavigateTo("Account/Profile");
            _shareSkill.OpenShareSkillsPage();
            _shareSkill.FillListing(_data.Listing);
            _shareSkill.Save();

            int count = _shareSkill.CountListingsWithTitle(_data.Listing.Title);
            string details = _shareSkill.GetListingsDetails();
            ShareSkillsAssertions.AssertListingSaved(count, _data.Listing.Title, details);

            // Switch to B and open A's listing
            LoginPage.LoginAs(Settings.SecondLogin.Username, Settings.SecondLogin.Password);
            _search.SearchByEnter(_data.Listing.Title);
            _search.OpenListing(_data.Listing.Title);
        }

        // B withdraws first so the listing can be deleted, then A cleans up
        protected override void AfterEachTest()
        {
            LoginPage.LoginAs(Settings.SecondLogin.Username, Settings.SecondLogin.Password);
            Nav.NavigateTo("Account/Profile");
            DataCleanupHooks.CleanSentRequests(_request);

            LoginPage.LoginAs(Settings.Login.Username, Settings.Login.Password);
            Nav.NavigateTo("Home/ListingManagement");
            DataCleanupHooks.CleanListings(_shareSkill, "TEARDOWN");
            DataCleanupHooks.CleanNotifications(_dropdown, "TEARDOWN");
        }

        // =====================================================================
        // REQUESTS (ManageRequestsComponent)
        // =====================================================================

        // NTF_TC_001 - B sends a request and sees "Request sent"
        [Test]
        [Category("Notifications")]
        public void NTF_TC_001_SendRequest_ShowsRequestSentToast()
        {
            _request.SendRequest(_data.RequestMessage);

            string toastText = Toasts.GetToastText();

            NotificationAssertions.AssertRequestSentToast(toastText, _data.RequestSentToast);
        }

        // NTF_TC_007 - Negative: B clicks No, so A gets no new notification
        [Test]
        [Category("Notifications")]
        public void NTF_TC_007_CancelRequest_NoNotification()
        {
            _request.CancelRequest(_data.RequestMessage);
            LoginPage.LoginAs(Settings.Login.Username, Settings.Login.Password);

            string count = _dropdown.GetUnreadCount();

            NotificationAssertions.AssertUnreadCount(count, "0");
        }

        // NTF_TC_008 - B withdraws the request, so A gets a withdraw notification
        [Test]
        [Category("Notifications")]
        public void NTF_TC_008_WithdrawRequest_NotifiesOwner()
        {
            _request.SendRequest(_data.RequestMessage);
            _request.WithdrawAllRequests();
            LoginPage.LoginAs(Settings.Login.Username, Settings.Login.Password);

            _dropdown.OpenDropdown();
            string messages = _dropdown.GetNotificationMessages();

            NotificationAssertions.AssertNotificationShown(messages, _data.SenderName + " " + _data.WithdrawText);
        }

        // =====================================================================
        // NOTIFICATION DROPDOWN (NotificationDropdownComponent)
        // =====================================================================

        // NTF_TC_002 - A sees an unread badge of 1
        [Test]
        [Category("Notifications")]
        [Category("Smoke")]
        public void NTF_TC_002_ReceivedRequest_ShowsUnreadBadge()
        {
            _request.SendRequest(_data.RequestMessage);
            LoginPage.LoginAs(Settings.Login.Username, Settings.Login.Password);

            string count = _dropdown.GetUnreadCount();

            NotificationAssertions.AssertUnreadCount(count, "1");
        }

        // NTF_TC_003 - A's dropdown shows the request from B
        [Test]
        [Category("Notifications")]
        public void NTF_TC_003_ReceivedRequest_ShowsNotificationMessage()
        {
            _request.SendRequest(_data.RequestMessage);
            LoginPage.LoginAs(Settings.Login.Username, Settings.Login.Password);

            _dropdown.OpenDropdown();
            string messages = _dropdown.GetNotificationMessages();

            NotificationAssertions.AssertNotificationShown(messages, _data.NotificationText + " " + _data.SenderName);
        }

        // NTF_TC_004 - Clicking the notification opens Received Requests
        [Test]
        [Category("Notifications")]
        public void NTF_TC_004_ClickNotification_OpensReceivedRequests()
        {
            _request.SendRequest(_data.RequestMessage);
            LoginPage.LoginAs(Settings.Login.Username, Settings.Login.Password);

            _dropdown.OpenDropdown();
            _dropdown.ClickNotification(_data.SenderName);

            NotificationAssertions.AssertUrlContains(Driver.Url, "ReceivedRequest");
        }

        // NTF_TC_005 - Mark all as read clears the unread badge
        [Test]
        [Category("Notifications")]
        public void NTF_TC_005_MarkAllAsRead_ClearsBadge()
        {
            _request.SendRequest(_data.RequestMessage);
            LoginPage.LoginAs(Settings.Login.Username, Settings.Login.Password);

            _dropdown.OpenDropdown();
            _dropdown.MarkAllAsRead();
            string count = _dropdown.GetUnreadCount();

            NotificationAssertions.AssertUnreadCount(count, "0");
        }

        // NTF_TC_006 - See All opens the Dashboard
        [Test]
        [Category("Notifications")]
        public void NTF_TC_006_SeeAll_OpensDashboard()
        {
            _request.SendRequest(_data.RequestMessage);
            LoginPage.LoginAs(Settings.Login.Username, Settings.Login.Password);

            _dropdown.OpenDropdown();
            _dropdown.ClickSeeAll();

            NotificationAssertions.AssertUrlContains(Driver.Url, "Dashboard");
        }

        // NTF_TC_017 - Negative: after deleting everything, the dropdown shows the empty message
        [Test]
        [Category("Notifications")]
        public void NTF_TC_017_NoNotifications_ShowsEmptyMessage()
        {
            SendRequestAndOpenDashboard();
            _dashboard.DeleteAllNotifications();

            _dropdown.OpenDropdown();
            string message = _dropdown.GetEmptyMessage();

            NotificationAssertions.AssertNotificationShown(message, _data.EmptyMessage);
        }

        // =====================================================================
        // DASHBOARD (DashboardComponent)
        // =====================================================================

        // NTF_TC_009 - Select all ticks every row
        [Test]
        [Category("Notifications")]
        [Category("Smoke")]
        public void NTF_TC_009_Dashboard_SelectAll_SelectsEveryRow()
        {
            SendRequestAndOpenDashboard();

            _dashboard.SelectAll();

            NotificationAssertions.AssertCount(_dashboard.GetSelectedCount(), _dashboard.GetRowCount(), "selected rows");
        }

        // NTF_TC_010 - Unselect all clears every tick
        [Test]
        [Category("Notifications")]
        public void NTF_TC_010_Dashboard_UnselectAll_ClearsSelection()
        {
            SendRequestAndOpenDashboard();

            _dashboard.SelectAll();
            _dashboard.UnselectAll();

            NotificationAssertions.AssertCount(_dashboard.GetSelectedCount(), 0, "selected rows");
        }

        // NTF_TC_011 - One row can be ticked, then unticked
        [Test]
        [Category("Notifications")]
        public void NTF_TC_011_Dashboard_ToggleRow_SelectsThenUnselects()
        {
            SendRequestAndOpenDashboard();

            _dashboard.ToggleRow(0);
            int afterSelect = _dashboard.GetSelectedCount();
            _dashboard.ToggleRow(0);
            int afterUnselect = _dashboard.GetSelectedCount();

            NotificationAssertions.AssertCount(afterSelect, 1, "selected rows after ticking");
            NotificationAssertions.AssertCount(afterUnselect, 0, "selected rows after unticking");
        }

        // NTF_TC_012 - Mark selection as read clears the unread badge
        [Test]
        [Category("Notifications")]
        public void NTF_TC_012_Dashboard_MarkSelectionAsRead_ClearsBadge()
        {
            SendRequestAndOpenDashboard();

            _dashboard.SelectAll();
            _dashboard.MarkSelectionAsRead();
            Nav.NavigateTo("Account/Dashboard");

            NotificationAssertions.AssertUnreadCount(_dropdown.GetUnreadCount(), "0");
        }

        // NTF_TC_013 - Deleting everything empties the list
        [Test]
        [Category("Notifications")]
        public void NTF_TC_013_Dashboard_DeleteAll_EmptiesList()
        {
            SendRequestAndOpenDashboard();

            _dashboard.DeleteAllNotifications();

            NotificationAssertions.AssertCount(_dashboard.GetRowCount(), 0, "notification rows");
        }

        // NTF_TC_014 - "Go to page" opens Received Requests
        [Test]
        [Category("Notifications")]
        public void NTF_TC_014_Dashboard_GoToPage_OpensReceivedRequests()
        {
            SendRequestAndOpenDashboard();

            _dashboard.ClickFirstGoToPage();

            NotificationAssertions.AssertUrlContains(Driver.Url, "ReceivedRequest");
        }

        // NTF_TC_015 - Load More shows more rows; Show Less goes back to the starting count.
        // Skipped until the app rejects unsafe input (known issue)
        // Known issue: Show Less leaves (total - 5) rows instead of 5. Expected to fail until fixed.
        [Test]
        [Category("Notifications")]
        [Category("KnownIssue")]
        [Ignore("Known issue: Dashboard Show Less leaves (total - 5) rows instead of 5. See Known Issues sheet.")]
        public void NTF_TC_015_Dashboard_LoadMoreThenShowLess()
        {
            for (int i = 0; i < 3; i++)
            {
                _search.SearchByEnter(_data.Listing.Title);
                _search.OpenListing(_data.Listing.Title);
                _request.SendRequest(_data.RequestMessage);
                _request.WithdrawAllRequests();
            }
            LoginPage.LoginAs(Settings.Login.Username, Settings.Login.Password);
            Nav.NavigateTo("Account/Dashboard");
            _dashboard.WaitForList();

            int before = _dashboard.GetRowCount();

            _dashboard.LoadMore();
            int afterLoadMore = _dashboard.GetRowCount();
            _dashboard.ShowLess();
            int afterShowLess = _dashboard.GetRowCount();

            NotificationAssertions.AssertMoreThan(afterLoadMore, before, "rows after Load More");
            NotificationAssertions.AssertCount(afterShowLess, before, "rows after Show Less");
        }

        // NTF_TC_016 - Negative: with nothing ticked, Unselect all / Delete / Mark as read are hidden
        [Test]
        [Category("Notifications")]
        public void NTF_TC_016_Dashboard_NoSelection_HidesSelectionActions()
        {
            SendRequestAndOpenDashboard();

            NotificationAssertions.AssertButtonHidden(_dashboard.IsUnselectAllShown(), "Unselect all");
            NotificationAssertions.AssertButtonHidden(_dashboard.IsDeleteSelectionShown(), "Delete selection");
            NotificationAssertions.AssertButtonHidden(_dashboard.IsMarkSelectionAsReadShown(), "Mark selection as read");
        }

        // =====================================================================
        // PRIVATE HELPERS
        // =====================================================================

        // B sends a request, then A logs in and opens the Dashboard notifications list
        private void SendRequestAndOpenDashboard()
        {
            _request.SendRequest(_data.RequestMessage);
            LoginPage.LoginAs(Settings.Login.Username, Settings.Login.Password);
            Nav.NavigateTo("Account/Dashboard");
            _dashboard.WaitForList();
        }
    }
}
