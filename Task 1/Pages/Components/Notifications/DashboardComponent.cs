// FILE: DashboardComponent.cs
// ROLE: Notifications list on the Dashboard page (/Account/Dashboard):
//       select, mark as read, delete, Go to page, Load More / Show Less.

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace Task1.Pages.Components.Notifications
{
    public class DashboardComponent
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators
        private By SelectAllBtnBy => By.XPath("//i[contains(@class,'mouse pointer')]/parent::div");
        private By UnselectAllBtnBy => By.CssSelector("div[data-tooltip='Unselect all']");
        private By DeleteSelectionBtnBy => By.XPath("//i[contains(@class,'trash')]/parent::div");
        private By MarkSelectionAsReadBtnBy => By.CssSelector("div[data-tooltip='Mark selection as read']");

        private By RowCheckboxesBy => By.CssSelector("input[type='checkbox']");
        private IWebElement RowCheckbox(int index) =>
            _driver.FindElement(By.CssSelector($"input[type='checkbox'][value='{index}']"));

        private By GoToPageLinksBy => By.XPath("//a[normalize-space(.)='Go to page']");

        private By LoadMoreBtnBy => By.XPath("//a[contains(@class,'button') and normalize-space(.)='Load More...']");
        private By ShowLessBtnBy => By.XPath("//a[contains(@class,'button') and normalize-space(.)='...Show Less']");

        public DashboardComponent(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            _wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        }

        // Call after Nav.NavigateTo("Account/Dashboard")
        public void WaitForList()
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(RowCheckboxesBy));
        }

        // =====================================================================
        // SELECT AND UNSELECT
        // =====================================================================

        public void SelectAll()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(SelectAllBtnBy)).Click();
        }

        public void UnselectAll()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(UnselectAllBtnBy)).Click();
        }

        public void ToggleRow(int index)
        {
            RowCheckbox(index).Click();
        }

        public int GetRowCount()
        {
            return _driver.FindElements(RowCheckboxesBy).Count;
        }

        public int GetSelectedCount()
        {
            int count = 0;
            foreach (var box in _driver.FindElements(RowCheckboxesBy))
            {
                if (box.Selected) count++;
            }
            return count;
        }

        // =====================================================================
        // MARK AS READ, DELETE, GO TO PAGE
        // =====================================================================

        public void MarkSelectionAsRead()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(MarkSelectionAsReadBtnBy)).Click();
        }

        public void DeleteSelection()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(DeleteSelectionBtnBy)).Click();
        }

        // Select all + Delete, one page at a time, until no rows are left
        public void DeleteAllNotifications()
        {
            while (true)
            {
                var rows = _driver.FindElements(RowCheckboxesBy);
                if (rows.Count == 0) break;

                var firstRow = rows[0];
                SelectAll();
                DeleteSelection();
                _wait.Until(ExpectedConditions.StalenessOf(firstRow));
            }
        }

        public void ClickFirstGoToPage()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(GoToPageLinksBy)).Click();
        }

        // =====================================================================
        // LOAD MORE AND SHOW LESS
        // =====================================================================

        public void LoadMore()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(LoadMoreBtnBy)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(ShowLessBtnBy));
        }

        public void ShowLess()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(ShowLessBtnBy)).Click();
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(ShowLessBtnBy));
        }

        // =====================================================================
        // BUTTON VISIBILITY (buttons may be removed, not just hidden)
        // =====================================================================

        public bool IsUnselectAllShown() => IsShown(UnselectAllBtnBy);
        public bool IsDeleteSelectionShown() => IsShown(DeleteSelectionBtnBy);
        public bool IsMarkSelectionAsReadShown() => IsShown(MarkSelectionAsReadBtnBy);

        private bool IsShown(By button)
        {
            var found = _driver.FindElements(button);
            return found.Count > 0 && found[0].Displayed;
        }
    }
}
