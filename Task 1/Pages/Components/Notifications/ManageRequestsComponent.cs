// FILE: ManageRequestsComponent.cs
// ROLE: Trade requests between the two users.
//       Send or cancel a request from a listing (/Home/ServiceDetail),
//       and withdraw from Manage Requests > Sent Requests.

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace Task1.Pages.Components.Notifications
{
    public class ManageRequestsComponent
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators: listing page (Request)
        // Message box: only the placeholder is unique, so match its start (^=)
        private By RequestMessageInputBy => By.CssSelector("textarea[placeholder^='I am interested']");
        // Request is a <div>, not a <button>. Its class has a double space, so use contains
        private IWebElement RequestBtn =>
            _driver.FindElement(By.XPath("//div[contains(@class,'teal') and contains(@class,'button') and contains(.,'Request')]"));

        // Confirmation popup ("You might not have the skills required for this trade.")
        private By ConfirmYesBtnBy => By.XPath("//button[contains(@class,'teal') and normalize-space(.)='Yes']");
        private By ConfirmNoBtnBy => By.XPath("//button[normalize-space(.)='No']");

        // Locators: Manage Requests > Sent Requests (Withdraw)
        private By ManageRequestsBy => By.XPath("//div[contains(@class,'dropdown') and contains(normalize-space(.),'Manage Requests')]");
        private By SentRequestsLinkBy => By.CssSelector("a.item[href='/Home/SentRequest']");
        // Rows in the Sent Requests table, used to know the table has loaded
        private By SentRequestRowsBy => By.XPath("//table//tbody/tr");
        // No popup. The button disappears once withdrawn (row shows "Withdrawn")
        private By WithdrawBtnBy => By.XPath("//button[contains(@class,'negative') and normalize-space(.)='Withdraw']");

        public ManageRequestsComponent(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            _wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        }

        // =====================================================================
        // SEND (listing page)
        // =====================================================================

        // Type a message, click Request, then Yes in the popup
        public void SendRequest(string message)
        {
            EnterMessageAndRequest(message);
            _wait.Until(ExpectedConditions.ElementToBeClickable(ConfirmYesBtnBy)).Click();
        }

        // Type a message, click Request, then No in the popup (request is not sent)
        public void CancelRequest(string message)
        {
            EnterMessageAndRequest(message);
            _wait.Until(ExpectedConditions.ElementToBeClickable(ConfirmNoBtnBy)).Click();
        }

        // =====================================================================
        // WITHDRAW (Sent Requests page)
        // =====================================================================

        // Manage Requests > Sent Requests, then wait for the table to load
        public void OpenSentRequests()
        {
            _driver.FindElement(ManageRequestsBy).Click();
            _wait.Until(ExpectedConditions.ElementToBeClickable(SentRequestsLinkBy)).Click();

            try
            {
                _wait.Until(ExpectedConditions.ElementExists(SentRequestRowsBy));
            }
            catch (WebDriverTimeoutException)
            {
                // No rows means no requests, so there is nothing to withdraw
            }
        }

        // Opens Sent Requests and clicks Withdraw on each pending request until none are left
        public void WithdrawAllRequests()
        {
            OpenSentRequests();

            while (_driver.FindElements(WithdrawBtnBy).Count > 0)
            {
                int before = _driver.FindElements(WithdrawBtnBy).Count;

                IWebElement withdraw = _driver.FindElement(WithdrawBtnBy);
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", withdraw);

                _wait.Until(d => d.FindElements(WithdrawBtnBy).Count < before);
            }
        }

        // =====================================================================
        // PRIVATE HELPERS
        // =====================================================================

        private void EnterMessageAndRequest(string message)
        {
            var messageBox = _wait.Until(ExpectedConditions.ElementIsVisible(RequestMessageInputBy));
            messageBox.SendKeys(message);
            RequestBtn.Click();
        }
    }
}
