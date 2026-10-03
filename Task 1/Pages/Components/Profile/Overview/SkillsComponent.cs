// FILE: SkillsComponent.cs
// ROLE: Component POM for Profile Overview > Skills tab.

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Task1.Support;

namespace Task1.Pages.Components.Profile.Overview
{
    public class SkillsComponent
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators
        private By SkillsTabBy => By.CssSelector("a[data-tab='second']");
        private By SkillsPaneBy => By.CssSelector("div[data-tab='second']");
        private By SkillRows => By.XPath("//div[@data-tab='second']//table//tbody/tr");
        private By RowDeleteIconBy => By.CssSelector("i.remove.icon");
        private By SkillFirstRowEditIconBy =>
            By.XPath("//div[@data-tab='second']//table//tbody/tr[1]//i[contains(@class,'write icon')]");

        private IWebElement SkillAddNewBtn => _driver.FindElement(By.XPath("//div[@data-tab='second']//div[contains(@class,'ui') and contains(@class,'button') and normalize-space(.)='Add New']"));
        private IWebElement SkillNameInput => _driver.FindElement(By.CssSelector("div[data-tab='second'] input[placeholder='Add Skill']"));
        private IWebElement SkillLevelSelect => _driver.FindElement(By.CssSelector("div[data-tab='second'] select[name='level']"));
        private IWebElement SkillAddBtn => _driver.FindElement(By.CssSelector("div[data-tab='second'] input.ui.teal.button[value='Add']"));
        private IWebElement SkillUpdateBtn => _driver.FindElement(By.CssSelector("div[data-tab='second'] input.ui.teal.button[value='Update']"));
        private IWebElement SkillCancelBtn => _driver.FindElement(By.CssSelector("div[data-tab='second'] input[value='Cancel']"));

        public SkillsComponent(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            _wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        }

        // =====================================================================
        // NAVIGATION
        // =====================================================================

        public void OpenSkillsTab()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(SkillsTabBy)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SkillsPaneBy));
        }

        // =====================================================================
        // ADD
        // =====================================================================

        // Normal add: waits for the row to appear
        public void AddSkill(string skill, string level)
        {
            SubmitSkillRaw(skill, level);
            WaitUntilSkillAppears(skill);
        }

        // Submits without waiting (duplicates, unsafe or massive input, where no row may appear)
        public void SubmitSkillRaw(string skill, string level)
        {
            OpenAddForm(skill);
            new SelectElement(SkillLevelSelect).SelectByText(level);
            SkillAddBtn.Click();
        }

        public void AddSkillWithoutName(string level)
        {
            OpenAddForm("");
            new SelectElement(SkillLevelSelect).SelectByText(level);
            SkillAddBtn.Click();
        }

        public void CancelAddSkill(string skill, string level)
        {
            OpenAddForm(skill);
            new SelectElement(SkillLevelSelect).SelectByText(level);
            SkillCancelBtn.Click();
        }

        // =====================================================================
        // UPDATE AND DELETE
        // =====================================================================

        public void UpdateSkill(string skill, string newLevel)
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(SkillFirstRowEditIconBy)).Click();
            SkillNameInput.Clear();
            SkillNameInput.SendKeys(skill);
            new SelectElement(SkillLevelSelect).SelectByText(newLevel);
            SkillUpdateBtn.Click();
            _wait.Until(d => CountSkillWithLevel(skill, newLevel) > 0);
        }

        public void DeleteSkill(string skill, string level)
        {
            foreach (var row in _driver.FindElements(SkillRows))
            {
                var cells = row.FindElements(By.TagName("td"));
                if (cells.Count < 2) continue;

                if (UiTextHelper.IsSameText(cells[0].Text, skill) && UiTextHelper.IsSameText(cells[1].Text, level))
                {
                    row.FindElement(RowDeleteIconBy).Click();
                    _wait.Until(d => CountSkillWithLevel(skill, level) == 0);
                    break;
                }
            }
        }

        // Used by the cleanup hooks. Deletes the first row until the table is empty.
        public void DeleteAllSkills()
        {
            while (true)
            {
                var rows = _driver.FindElements(SkillRows);
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

                _wait.Until(d => d.FindElements(SkillRows).Count < before);
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

        public class SkillItem
        {
            public string Skill { get; set; }
            public string Level { get; set; }
        }

        public List<SkillItem> GetSkills()
        {
            List<SkillItem> list = new();
            foreach (var row in _driver.FindElements(SkillRows))
            {
                var cells = row.FindElements(By.TagName("td"));
                var item = new SkillItem();

                if (cells.Count > 0) item.Skill = cells[0].Text.Trim();
                if (cells.Count > 1) item.Level = cells[1].Text.Trim();

                list.Add(item);
            }
            return list;
        }

        public int CountSkillWithLevel(string skill, string level)
        {
            int count = 0;
            foreach (var row in GetSkills())
                if (UiTextHelper.IsSameText(row.Skill, skill) && UiTextHelper.IsSameText(row.Level, level))
                    count++;
            return count;
        }

        // Text summary of the table for assertion messages, e.g. "C#:Expert, Git:Intermediate"
        public string GetSkillsDetails()
        {
            string details = "";
            foreach (var r in GetSkills())
            {
                if (details != "") details += ", ";
                details += r.Skill + ":" + r.Level;
            }
            return details;
        }

        public void WaitUntilSkillAppears(string skill)
        {
            _wait.Until(driver =>
            {
                foreach (var row in driver.FindElements(SkillRows))
                {
                    var cells = row.FindElements(By.TagName("td"));
                    if (cells.Count > 0 && UiTextHelper.IsSameText(cells[0].Text, skill))
                        return true;
                }
                return false;
            });
        }

        // =====================================================================
        // PRIVATE HELPERS
        // =====================================================================

        // Clicks Add New and types the skill name (leave empty to skip typing)
        private void OpenAddForm(string skill)
        {
            SkillAddNewBtn.Click();
            SkillNameInput.Clear();
            if (skill != "")
                SkillNameInput.SendKeys(skill);
        }

        
    }
}
