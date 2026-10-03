// FILE: LanguagesComponent.cs
// ROLE: Component POM for Profile Overview > Languages tab.

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Task1.Support;

namespace Task1.Pages.Components.Profile.Overview
{
    public class LanguagesComponent
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators
        private By LangRowsBy => By.XPath("//div[@data-tab='first']//table//tbody/tr");
        private By RowDeleteIconBy => By.CssSelector("i.remove.icon");
        private By LangAddNewBtnBy => By.XPath("//div[@data-tab='first']//div[contains(@class,'ui') and contains(@class,'button') and normalize-space(.)='Add New']");
        private By LangFirstRowEditIconBy => By.XPath("//div[@data-tab='first']//table//tbody/tr[1]//i[contains(@class,'write icon')]");

        private IWebElement LangAddNewBtn => _driver.FindElement(LangAddNewBtnBy);
        private IWebElement LangInput => _driver.FindElement(By.CssSelector("div[data-tab='first'] input[placeholder='Add Language']"));
        private IWebElement LangLevelSelect => _driver.FindElement(By.CssSelector("div[data-tab='first'] select[name='level']"));
        private IWebElement LangAddBtn => _driver.FindElement(By.CssSelector("div[data-tab='first'] input.ui.teal.button[value='Add']"));
        private IWebElement LangUpdateBtn => _driver.FindElement(By.CssSelector("div[data-tab='first'] input[value='Update']"));
        private IWebElement LangCancelBtn => _driver.FindElement(By.CssSelector("div[data-tab='first'] input[value='Cancel']"));

        public LanguagesComponent(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            _wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
        }

        // =====================================================================
        // ADD
        // =====================================================================

        // Normal add: waits for the row to appear
        public void AddLanguage(string language, string level)
        {
            SubmitLanguageRaw(language, level);
            WaitUntilLanguageAppears(language);
        }

        // Submits without waiting (duplicates, unsafe or massive input, where no row may appear)
        public void SubmitLanguageRaw(string language, string level)
        {
            OpenAddForm(language);
            new SelectElement(LangLevelSelect).SelectByText(level);
            LangAddBtn.Click();
        }

        // Tries to pick a level that may not exist, submits anyway.
        // Returns true if the level existed in the dropdown.
        public bool AddLanguageAllowingInvalidLevel(string language, string level)
        {
            OpenAddForm(language);
            bool levelSelected = SelectLevelIfExists(level);
            LangAddBtn.Click();
            return levelSelected;
        }

        public void AddLanguageWithoutName(string level)
        {
            OpenAddForm("");
            new SelectElement(LangLevelSelect).SelectByText(level);
            LangAddBtn.Click();
        }

        public void CancelAddLanguage(string language, string level)
        {
            OpenAddForm(language);
            new SelectElement(LangLevelSelect).SelectByText(level);
            LangCancelBtn.Click();
        }

        // =====================================================================
        // UPDATE AND DELETE
        // =====================================================================

        public void UpdateLanguage(string language, string newLevel)
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(LangFirstRowEditIconBy)).Click();
            LangInput.Clear();
            LangInput.SendKeys(language);
            new SelectElement(LangLevelSelect).SelectByText(newLevel);
            LangUpdateBtn.Click();
            _wait.Until(d => CountLanguageWithLevel(language, newLevel) > 0);
        }

        public void DeleteLanguage(string language, string level)
        {
            foreach (var row in _driver.FindElements(LangRowsBy))
            {
                var cells = row.FindElements(By.TagName("td"));
                if (cells.Count < 2) continue;

                if (UiTextHelper.IsSameText(cells[0].Text, language) && UiTextHelper.IsSameText(cells[1].Text, level))
                {
                    row.FindElement(RowDeleteIconBy).Click();
                    _wait.Until(d => CountLanguageWithLevel(language, level) == 0);
                    break;
                }
            }
        }

        // Used by the cleanup hooks. Deletes the first row until the table is empty.
        public void DeleteAllLanguages()
        {
            while (true)
            {
                var rows = _driver.FindElements(LangRowsBy);
                if (rows.Count == 0)
                    break;

                var before = rows.Count;

                try
                {
                    rows[0].FindElement(RowDeleteIconBy).Click();
                }
                catch (StaleElementReferenceException)
                {
                    // The table re-rendered before the click. Read the rows again.
                    continue;
                }

                _wait.Until(d => d.FindElements(LangRowsBy).Count < before);
            }
        }

        // =====================================================================
        // ALERTS
        // =====================================================================

        // Returns the alert text and accepts it, or null if no alert appears
        public string TryGetAlertText(int seconds = 2)
        {
            try
            {
                var alert = new WebDriverWait(_driver, TimeSpan.FromSeconds(seconds))
                    .Until(ExpectedConditions.AlertIsPresent());
                string text = alert.Text;
                alert.Accept();
                return text;
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        // =====================================================================
        // READING THE TABLE
        // =====================================================================

        public class LanguageItem
        {
            public string Language { get; set; }
            public string Level { get; set; }
        }

        public List<LanguageItem> GetLanguages()
        {
            List<LanguageItem> list = new();
            foreach (var row in _driver.FindElements(LangRowsBy))
            {
                var cells = row.FindElements(By.TagName("td"));
                var item = new LanguageItem();

                if (cells.Count > 0) item.Language = cells[0].Text.Trim();
                if (cells.Count > 1) item.Level = cells[1].Text.Trim();

                list.Add(item);
            }
            return list;
        }

        public int CountLanguageWithLevel(string language, string level)
        {
            int count = 0;
            foreach (var row in GetLanguages())
                if (UiTextHelper.IsSameText(row.Language, language) && UiTextHelper.IsSameText(row.Level, level))
                    count++;
            return count;
        }

        // Text summary of the table for assertion messages, e.g. "English:Fluent, Hindi:Basic"
        public string GetLanguagesDetails()
        {
            string details = "";
            foreach (var r in GetLanguages())
            {
                if (details != "") details += ", ";
                details += r.Language + ":" + r.Level;
            }
            return details;
        }

        public bool IsAddNewButtonDisplayed()
        {
            foreach (var el in _driver.FindElements(LangAddNewBtnBy))
                if (el.Displayed) return true;
            return false;
        }

        public void WaitUntilLanguageAppears(string language)
        {
            _wait.Until(driver =>
            {
                foreach (var row in driver.FindElements(LangRowsBy))
                {
                    var cells = row.FindElements(By.TagName("td"));
                    if (cells.Count > 0 && UiTextHelper.IsSameText(cells[0].Text, language))
                        return true;
                }
                return false;
            });
        }

        // =====================================================================
        // PRIVATE HELPERS
        // =====================================================================

        // Clicks Add New and types the language name (leave empty to skip typing)
        private void OpenAddForm(string language)
        {
            LangAddNewBtn.Click();
            LangInput.Clear();
            if (language != "")
                LangInput.SendKeys(language);
        }

        private bool SelectLevelIfExists(string level)
        {
            try
            {
                new SelectElement(LangLevelSelect).SelectByText(level);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        
        
    }
}