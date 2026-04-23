// FILE: LoginPage.cs
// ROLE: Authentication POM — opens Sign In modal, performs login,
//       and surfaces validation signals.

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Task1.Support;
using SeleniumExtras.WaitHelpers;
using System;

namespace Task1.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly NavigationHelper _nav;

        // ── Locators ─────────────────────────────────────────────────────────
        private readonly By SignInLink =
            By.XPath("//a[@class='item' and text()='Sign In']");

        private readonly By UsernameField =
            By.CssSelector("input[name='email'][placeholder='Email address']");

        private readonly By PasswordField =
            By.CssSelector("input[type='password']");

        private readonly By LoginButton =
            By.XPath("//button[normalize-space()='Login']");

        private readonly By SignOutButton =
            By.XPath("//button[normalize-space()='Sign Out'] | //a[normalize-space()='Sign Out']");

        private readonly By EmailErrorPrompt =
            By.XPath("//div[text()='Please enter a valid email address']");

        private readonly By PasswordErrorPrompt =
            By.XPath("//div[text()='Password must be at least 6 characters']");

        private readonly By ErrorToast =
            By.XPath("//div[@class='ns-box-inner' and text()='Confirm your email']");

        // ── Constructor ───────────────────────────────────────────────────────
        public LoginPage(IWebDriver driver, NavigationHelper nav)
        {
            _driver = driver;
            _nav    = nav;
            _wait   = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        // ── Actions ───────────────────────────────────────────────────────────
        public void OpenSignIn()
        {
            _nav.NavigateTo("/");
            _driver.Manage().Window.Maximize();
            _wait.Until(ExpectedConditions.ElementToBeClickable(SignInLink)).Click();
        }

        public void Login(string username, string password)
        {
            var userEl = _wait.Until(ExpectedConditions.ElementIsVisible(UsernameField));
            userEl.Clear();
            userEl.SendKeys(username);

            var passEl = _wait.Until(ExpectedConditions.ElementIsVisible(PasswordField));
            passEl.Clear();
            passEl.SendKeys(password);

            _wait.Until(ExpectedConditions.ElementToBeClickable(LoginButton)).Click();
        }

        public void WaitUntilLoggedIn(int seconds = 5)
        {
            new WebDriverWait(_driver, TimeSpan.FromSeconds(seconds))
                .Until(_ => IsLoggedIn());
        }

        // ── Queries ───────────────────────────────────────────────────────────
        public bool IsLoggedIn()
        {
            try { return _driver.FindElements(SignOutButton).Count > 0; }
            catch { return false; }
        }

        public string GetEmailError()
        {
            try
            {
                var els = _driver.FindElements(EmailErrorPrompt);
                return els.Count > 0 && els[0].Displayed ? els[0].Text.Trim() : string.Empty;
            }
            catch { return string.Empty; }
        }

        public string GetPasswordError()
        {
            try
            {
                var els = _driver.FindElements(PasswordErrorPrompt);
                return els.Count > 0 && els[0].Displayed ? els[0].Text.Trim() : string.Empty;
            }
            catch { return string.Empty; }
        }

        public string GetPopupError()
        {
            try
            {
                var els = _driver.FindElements(ErrorToast);
                return els.Count > 0 && els[0].Displayed ? els[0].Text.Trim() : string.Empty;
            }
            catch { return string.Empty; }
        }
    }
}
