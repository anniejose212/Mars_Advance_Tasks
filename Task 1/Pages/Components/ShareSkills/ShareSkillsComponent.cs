// FILE: ShareSkillsComponent.cs
// ROLE: Share Skill form (/Home/ServiceListing) and Manage Listings (/Home/ListingManagement):
//       fill and save a listing, check field limits, find saved listings, delete all for cleanup.

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using Task1.TestDataModel;

namespace Task1.Pages.Components.ShareSkills
{
    public class ShareSkillsComponent
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators: Share Skill form
        private By ShareSkillsBtnBy => By.CssSelector("a.ui.green.button[href='/Home/ServiceListing']");
        private By TitleInputBy => By.CssSelector("input[name='title']");
        private By SubCategorySelectBy => By.CssSelector("select[name='subcategoryId']");
        private By CreditChargeInputBy => By.CssSelector("input[name='charge']");

        private IWebElement TitleInput => _driver.FindElement(TitleInputBy);
        private IWebElement DescriptionInput => _driver.FindElement(By.CssSelector("textarea[name='description']"));
        private IWebElement CategorySelect => _driver.FindElement(By.CssSelector("select[name='categoryId']"));
        private IWebElement AddTagsInput => _driver.FindElement(By.XPath("(//input[contains(@class,'ReactTags__tagInputField')])[1]"));
        private IWebElement SkillExchangeTagInput => _driver.FindElement(By.XPath("(//input[contains(@class,'ReactTags__tagInputField')])[2]"));

        // Save and Cancel are <input type="button">
        private IWebElement SaveButton => _driver.FindElement(By.CssSelector("input.ui.teal.button[value='Save']"));
        private IWebElement CancelButton => _driver.FindElement(By.CssSelector("input.ui.button[value='Cancel']"));

        // Locators: Manage Listings. Delete is the Actions button holding i.remove.icon
        private By ListingRows => By.CssSelector("table.ui.striped.table tbody tr");
        private By RowDeleteBtnBy => By.XPath(".//button[i[contains(@class,'remove')]]");
        private By ConfirmYesBtnBy => By.XPath("//div[contains(@class,'ui') and contains(@class,'modal')]//button[normalize-space(.)='Yes']");

        public ShareSkillsComponent(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            _wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        }

        // =====================================================================
        // NAVIGATION
        // =====================================================================

        public void OpenShareSkillsPage()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(ShareSkillsBtnBy)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(TitleInputBy));
        }

        // =====================================================================
        // FORM
        // =====================================================================

        // Fills every section from one JSON entry. Empty values are skipped.
        public void FillListing(ShareSkillListing listing)
        {
            TypeInto(TitleInput, listing.Title);
            TypeInto(DescriptionInput, listing.Description);
            SelectCategory(listing.Category, listing.SubCategory);
            AddTags(AddTagsInput, listing.Tags);
            SelectServiceType(listing.ServiceType);
            SelectLocationType(listing.LocationType);
            SelectSkillTrade(listing.SkillTrade, listing.SkillExchangeTags, listing.Credit);
            SelectActive(listing.Active);
        }

        public void Save() => SaveButton.Click();

        public void Cancel() => CancelButton.Click();

        // Types the input into one field and returns how many characters the field kept
        public int EnterAndGetLength(string field, string input)
        {
            IWebElement target;

            if (field == "Title")
                target = TitleInput;
            else if (field == "Description")
                target = DescriptionInput;
            else if (field == "Credit")
            {
                ClickRadio("skillTrades", "false");
                target = _wait.Until(ExpectedConditions.ElementIsVisible(CreditChargeInputBy));
            }
            else
                throw new ArgumentException($"Unknown field: {field}");

            ReplaceText(target, input);
            return target.GetAttribute("value").Length;
        }

        // =====================================================================
        // MANAGE LISTINGS
        // =====================================================================

        // After a successful Save the app redirects to Manage Listings and the success
        // toast vanishes with the old page, so the saved row is checked instead.
        public int CountListingsWithTitle(string title)
        {
            _wait.Until(ExpectedConditions.UrlContains("ListingManagement"));
            _wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(ListingRows));

            By rowWithTitle = By.XPath($"//table[contains(@class,'striped')]//tbody/tr[td[normalize-space(.)='{title}']]");
            return _driver.FindElements(rowWithTitle).Count;
        }

        // One line per listing, cells separated by " | ", for TestContext logging
        public string GetListingsDetails()
        {
            string details = "";
            foreach (var row in _driver.FindElements(ListingRows))
            {
                if (details != "") details += "; ";
                details += row.Text.Trim().Replace("\r\n", " | ").Replace("\n", " | ");
            }
            return details;
        }

        // Used by the cleanup hooks. Call on Home/ListingManagement. Deletes rows one at a time.
        public void DeleteAllListings()
        {
            while (true)
            {
                var rows = _driver.FindElements(ListingRows);
                if (rows.Count == 0) break;

                var firstRow = rows[0];
                firstRow.FindElement(RowDeleteBtnBy).Click();
                _wait.Until(ExpectedConditions.ElementToBeClickable(ConfirmYesBtnBy)).Click();

                // The deleted row leaves the DOM, so wait for it to go stale before the next loop
                _wait.Until(ExpectedConditions.StalenessOf(firstRow));
            }
        }

        // =====================================================================
        // FORM SECTIONS
        // =====================================================================

        private void SelectCategory(string category, string subCategory)
        {
            if (string.IsNullOrEmpty(category)) return;
            new SelectElement(CategorySelect).SelectByText(category);

            if (string.IsNullOrEmpty(subCategory)) return;
            var sub = _wait.Until(ExpectedConditions.ElementIsVisible(SubCategorySelectBy));
            new SelectElement(sub).SelectByText(subCategory);
        }

        private void SelectServiceType(string serviceType)
        {
            if (serviceType == "Hourly") ClickRadio("serviceType", "0");
            else if (serviceType == "One-off") ClickRadio("serviceType", "1");
        }

        private void SelectLocationType(string locationType)
        {
            if (locationType == "On-site") ClickRadio("locationType", "0");
            else if (locationType == "Online") ClickRadio("locationType", "1");
        }

        private void SelectSkillTrade(string skillTrade, List<string> exchangeTags, string credit)
        {
            if (skillTrade == "Skill-exchange")
            {
                ClickRadio("skillTrades", "true");
                AddTags(SkillExchangeTagInput, exchangeTags);
            }
            else if (skillTrade == "Credit")
            {
                ClickRadio("skillTrades", "false");
                var charge = _wait.Until(ExpectedConditions.ElementIsVisible(CreditChargeInputBy));
                if (!string.IsNullOrEmpty(credit)) ReplaceText(charge, credit);
            }
        }

        private void SelectActive(string active)
        {
            if (active == "Active") ClickRadio("isActive", "true");
            else if (active == "Hidden") ClickRadio("isActive", "false");
        }

        // =====================================================================
        // PRIVATE HELPERS
        // =====================================================================

        // The radio input is transparent (Semantic UI), so a normal click lands on its wrapper.
        // Click the input itself with JavaScript.
        private void ClickRadio(string name, string value)
        {
            var radio = _driver.FindElement(By.CssSelector($"input[name='{name}'][value='{value}']"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", radio);
        }

        // React tag input: type each tag and press Enter
        private static void AddTags(IWebElement input, List<string> tags)
        {
            if (tags == null) return;
            foreach (var tag in tags)
            {
                input.SendKeys(tag);
                input.SendKeys(Keys.Enter);
            }
        }

        private static void TypeInto(IWebElement input, string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            ReplaceText(input, text);
        }

        // Ctrl+A then type: Clear() does not always update React state
        private static void ReplaceText(IWebElement input, string text)
        {
            input.SendKeys(Keys.Control + "a");
            input.SendKeys(text);
            input.SendKeys(Keys.Tab);
        }
    }
}
