// FILE: NotificationDropdownComponent.cs
// ROLE: Top bar "Notification" dropdown: unread badge, notification list,
//       Mark all as read and See All.

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace Task1.Pages.Components.Notifications
{
    public class NotificationDropdownComponent
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators
        // The toggle is found through its list icon: matching the word "Notification"
        // would also match "You have no notifications" inside the menu.
        private IWebElement NotificationToggle =>
            _driver.FindElement(By.XPath("//div[contains(@class,'dropdown') and i[contains(@class,'list')]]"));
        private By NotificationMenuBy =>
            By.XPath("//div[contains(@class,'dropdown') and i[contains(@class,'list')]]/div[contains(@class,'menu')]");
        private By UnreadBadgeBy => By.CssSelector("div.floating.ui.blue.label");

        private By MarkAllAsReadBy => By.XPath("//a[normalize-space(.)='Mark all as read']");
        private IWebElement SeeAllLink => _driver.FindElement(By.XPath("//a[normalize-space(.)='See All...']"));

        // Each notification is a div.item with a name attribute (the notification id)
        private By NotificationMessagesBy => By.CssSelector("div.item[name] a.link.header div.content");
        private IWebElement NotificationWithText(string text) =>
            _driver.FindElement(By.XPath($"//div[@name]//a[contains(@class,'header') and contains(.,'{text}')]"));

        // Empty state. @class='item' exactly, so real notification items (item active selected) never match
        private By NoNotificationsBy => By.XPath("//div[@class='item' and normalize-space(.)='You have no notifications']");

        public NotificationDropdownComponent(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            _wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        }

        // =====================================================================
        // OPEN AND READ
        // =====================================================================

        public void OpenDropdown()
        {
            NotificationToggle.Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(NotificationMenuBy));
        }

        // "0" when there is no badge
        public string GetUnreadCount()
        {
            var badges = _driver.FindElements(UnreadBadgeBy);
            if (badges.Count == 0) return "0";
            return badges[0].Text.Trim();
        }

        // All notification messages in the dropdown, for assertions and logging
        public string GetNotificationMessages()
        {
            string details = "";
            foreach (var message in _driver.FindElements(NotificationMessagesBy))
            {
                if (details != "") details += "; ";
                details += message.Text.Trim();
            }
            return details;
        }

        public string GetEmptyMessage()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(NoNotificationsBy)).Text.Trim();
        }

        // =====================================================================
        // ACTIONS
        // =====================================================================

        public void ClickNotification(string text)
        {
            NotificationWithText(text).Click();
        }

        // Waits for the badge to go, otherwise the next read can hit a stale badge
        public void MarkAllAsRead()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(MarkAllAsReadBy)).Click();
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(UnreadBadgeBy));
        }

        public void ClickSeeAll()
        {
            SeeAllLink.Click();
        }

        // =====================================================================
        // CLEANUP
        // =====================================================================

        // Clears the unread badge so each test starts at 0.
        // The link only exists when there are notifications.
        public void MarkAllAsReadIfAny()
        {
            OpenDropdown();
            if (_driver.FindElements(MarkAllAsReadBy).Count > 0)
            {
                MarkAllAsRead();
            }
        }
    }
}
