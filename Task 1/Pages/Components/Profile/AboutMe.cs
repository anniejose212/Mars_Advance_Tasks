// FILE: AboutMe.cs
// ROLE: Component POM for the Profile "About Me" panel: name, Location,
//       Availability, Hours and Earn Target. Toasts are read by the tests through ToastHelper.

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace Task1.Pages.Components.Profile
{
    public class AboutMe
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators
        // Each profile row is a div.item containing a <strong> label
        private static string Item(string label) =>
            $"//div[contains(@class,'item')][.//strong[text()='{label}']]";

        private static By EditIconBy(string label) =>
            By.XPath(Item(label) + "//i[contains(@class,'write icon')]");

        private static By ValueBy(string label) =>
            By.XPath(Item(label) + "//div[contains(@class,'right floated content')]/span");

        private IWebElement AvailabilitySelect => _driver.FindElement(By.CssSelector("select[name='availabiltyType']"));
        private IWebElement HoursSelect => _driver.FindElement(By.CssSelector("select[name='availabiltyHour']"));
        private IWebElement EarnTargetSelect => _driver.FindElement(By.CssSelector("select[name='availabiltyTarget']"));

        private readonly By LocationLabelBy = By.XPath("//strong[text()='Location']");

        // Name editor: exact class match, so other elements with "title" in their class never match
        private By NameTitleBy => By.XPath("//div[(@class='title' or @class='title active') and i[contains(@class,'dropdown')]]");
        private By NameTitleArrowBy => By.XPath("//div[(@class='title' or @class='title active')]/i[contains(@class,'dropdown')]");
        private By FirstNameInputBy => By.CssSelector("input[name='firstName']");
        private By LastNameInputBy => By.CssSelector("input[name='lastName']");
        private By SaveNameBtnBy => By.XPath("//input[@name='lastName']/following::button[normalize-space(.)='Save'][1]");

        public AboutMe(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            _wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        }

        // =====================================================================
        // AVAILABILITY, HOURS, EARN TARGET
        // =====================================================================

        // Each selection saves straight away and shows a toast.
        // Read it with Toasts.GetToastText() before the next edit.
        public void SelectAvailability(string value)
        {
            ClickEdit("Availability");
            new SelectElement(AvailabilitySelect).SelectByText(value);
        }

        public void SelectHours(string value)
        {
            ClickEdit("Hours");
            new SelectElement(HoursSelect).SelectByText(value);
        }

        public void SelectEarnTarget(string value)
        {
            ClickEdit("Earn Target");
            new SelectElement(EarnTargetSelect).SelectByText(value);
        }

        public string GetSelectedAvailability() => GetValue("Availability");
        public string GetSelectedHours() => GetValue("Hours");
        public string GetSelectedEarnTarget() => GetValue("Earn Target");

        // Used by the cleanup hook: clicking the value closes the row without saving
        public void ClickAvailabilityValue() => ClickValue("Availability");
        public void ClickHoursValue() => ClickValue("Hours");
        public void ClickEarnTargetValue() => ClickValue("Earn Target");

        // =====================================================================
        // LOCATION
        // =====================================================================

        public bool IsLocationVisible() =>
            _wait.Until(ExpectedConditions.ElementIsVisible(LocationLabelBy)).Displayed;

        public bool IsLocationEditable() =>
            _driver.FindElements(By.XPath(Item("Location") + "//input")).Count > 0;

        // =====================================================================
        // NAME
        // =====================================================================

        // Opens the name editor (arrow next to the name), types both names, clicks Save
        public void UpdateName(string firstName, string lastName)
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(NameTitleArrowBy)).Click();

            var first = _wait.Until(ExpectedConditions.ElementIsVisible(FirstNameInputBy));
            first.Clear();
            first.SendKeys(firstName);

            var last = _driver.FindElement(LastNameInputBy);
            last.Clear();
            last.SendKeys(lastName);

            _driver.FindElement(SaveNameBtnBy).Click();
        }

        public string GetDisplayedName()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(NameTitleBy)).Text.Trim();
        }

        // =====================================================================
        // PRIVATE HELPERS
        // =====================================================================

        private void ClickEdit(string label) =>
            _wait.Until(ExpectedConditions.ElementToBeClickable(EditIconBy(label))).Click();

        private string GetValue(string label) =>
            _driver.FindElement(ValueBy(label)).Text.Trim();

        private void ClickValue(string label) =>
            _driver.FindElement(ValueBy(label)).Click();
    }
}