// FILE: SearchSkillsComponent.cs
// ROLE: Top-bar Search Skills box + search results page (categories, filters, user search, result cards).

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace Task1.Pages.Components.SearchSkills
{
    public class SearchSkillsComponent
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators
        // Top bar. [1] = top bar box, [2] = second box in the results page left panel
        private IWebElement SearchInput => _driver.FindElement(By.XPath("(//input[@placeholder='Search skills'])[1]"));
        private IWebElement SearchIcon => _driver.FindElement(By.XPath("(//input[@placeholder='Search skills'])[1]/following-sibling::i[contains(@class,'search')]"));

        // Result cards
        private By ResultTitlesBy => By.CssSelector("a.service-info p.row-padded");
        private IWebElement ResultLink(string title) =>
            _driver.FindElement(By.XPath($"//a[contains(@class,'service-info') and p[normalize-space(.)='{title}']]"));

        // Left panel. text() skips the count <span> inside the link
        private IWebElement AllCategoriesLink =>
            _driver.FindElement(By.XPath("//a[contains(@class,'item') and .//text()[normalize-space(.)='All Categories']]"));
        private IWebElement CategoryItem(string category) =>
            _driver.FindElement(By.XPath($"//a[contains(@class,'category') and normalize-space(text())='{category}']"));
        private IWebElement CategoryCount(string category) =>
            _driver.FindElement(By.XPath($"//a[contains(@class,'category') and normalize-space(text())='{category}']/span"));
        private IWebElement SubCategoryItem(string subCategory) =>
            _driver.FindElement(By.XPath($"//a[contains(@class,'subcategory') and normalize-space(text())='{subCategory}']"));

        // Online / Onsite / ShowAll buttons
        private IWebElement LocationFilterBtn(string text) =>
            _driver.FindElement(By.XPath($"//button[contains(@class,'ui button') and normalize-space(.)='{text}']"));

        // "Search user" box and its suggestions.
        // Suggestion uses @class='result' exactly, so it does not match the outer div.results
        private IWebElement SearchUserInput => _driver.FindElement(By.CssSelector("input.prompt[placeholder='Search user']"));
        private By UserSuggestionBy(string name) =>
            By.XPath($"//div[@class='result' and .//span[normalize-space(.)='{name}']]");

        private By NoResultsMessageBy => By.XPath("//h3[contains(.,'No results found')]");

        public SearchSkillsComponent(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            _wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        }

        // =====================================================================
        // SEARCH
        // =====================================================================

        public void SearchByEnter(string text)
        {
            SearchInput.SendKeys(text);
            SearchInput.SendKeys(Keys.Enter);
        }

        public void SearchByIcon(string text)
        {
            SearchInput.SendKeys(text);
            SearchIcon.Click();
        }

        // Only on the results page: type the name, then pick it from the suggestions
        public void SearchUser(string name)
        {
            SearchUserInput.SendKeys(name);
            _wait.Until(ExpectedConditions.ElementToBeClickable(UserSuggestionBy(name))).Click();
        }

        // =====================================================================
        // FILTERS
        // =====================================================================

        public void SelectAllCategories()
        {
            AllCategoriesLink.Click();
        }

        public void SelectCategory(string category)
        {
            CategoryItem(category).Click();
        }

        public void SelectSubCategory(string subCategory)
        {
            SubCategoryItem(subCategory).Click();
        }

        public void SelectLocationFilter(string text)
        {
            IWebElement button = LocationFilterBtn(text);

            // When the results are empty, the results column overlaps the filter
            // buttons in the test browser, so a normal click lands on the column.
            // Works by hand, so click the button directly.
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", button);
        }

        // =====================================================================
        // RESULTS
        // =====================================================================

        public void OpenListing(string title)
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(ResultTitlesBy));
            ResultLink(title).Click();
        }

        // Waits for result cards, then counts the ones with this exact title
        public int CountResultsWithTitle(string title)
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(ResultTitlesBy));

            int count = 0;
            foreach (var result in _driver.FindElements(ResultTitlesBy))
            {
                if (result.Text.Trim() == title) count++;
            }
            return count;
        }

        public string GetNoResultsMessage()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(NoResultsMessageBy)).Text.Trim();
        }

        public string GetCategoryCount(string category)
        {
            return CategoryCount(category).Text.Trim();
        }

        // All result titles, for TestContext logging
        public string GetResultsDetails()
        {
            string details = "";
            foreach (var result in _driver.FindElements(ResultTitlesBy))
            {
                if (details != "") details += ", ";
                details += result.Text.Trim();
            }
            return details;
        }
    }
}
