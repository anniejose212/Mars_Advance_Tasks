// FILE: LoginPage.cs
// ROLE: Authentication POM — opens Sign In, performs login, surfaces validation/lockout signals.

using Task2.Support;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Task2.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly NavigationHelper _nav;
        public IWebDriver Driver => _driver;

        // Locators
        private readonly By SignInLink = By.XPath("//a[@class='item' and text()='Sign In']");
        private readonly By UsernameField = By.CssSelector("input[name='email'][placeholder='Email address']");
        private readonly By PasswordField = By.CssSelector("input[type='password']");
        private readonly By LoginButton = By.XPath("//button[normalize-space()='Login']");
        private readonly By SuccessButton = By.XPath("//button[normalize-space()='Sign Out' ] | //a[normalize-space()='Sign Out' ]");
        private readonly By PasswordErrorPrompt = By.XPath("//div[text()='Password must be at least 6 characters']");
        private readonly By EmailErrorPrompt = By.XPath("//div[text()='Please enter a valid email address']");
        private readonly By ErrorToast = By.XPath("//div[@class='ns-box-inner' and text()='Confirm your email']");
        private readonly By LockoutMessage = By.XPath("//div[contains(text(),'too many attempts') or contains(text(),'locked')]");
        private readonly By ActiveDimmer = By.CssSelector("div.ui.page.modals.dimmer.active");

        public LoginPage(IWebDriver driver, NavigationHelper nav)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
            _nav = nav;
        }

        public void OpenSignIn()
        {
            _nav.NavigateTo("/");
            _driver.Manage().Window.Maximize();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(ActiveDimmer));
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

        public string GetSuccessMessage()
        {
            return _wait.Until(d => d.FindElement(SuccessButton)).Text;
        }

        public string GetEmailError()
        {
            try
            {
                var list = _driver.FindElements(EmailErrorPrompt);
                if (list.Count > 0 && list[0].Displayed)
                    return list[0].Text.Trim();
            }
            catch { }
            return string.Empty;
        }

        public string GetPasswordError()
        {
            try
            {
                var list = _driver.FindElements(PasswordErrorPrompt);
                if (list.Count > 0 && list[0].Displayed)
                    return list[0].Text.Trim();
            }
            catch { }
            return string.Empty;
        }

        public string GetPopupError()
        {
            try
            {
                var list = _driver.FindElements(ErrorToast);
                if (list.Count > 0 && list[0].Displayed)
                    return list[0].Text.Trim();
            }
            catch { }
            return string.Empty;
        }

        public bool IsLoggedIn()
        {
            try
            {
                return _driver.FindElements(SuccessButton).Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public string TryGetSuccess()
        {
            try
            {
                var els = _driver.FindElements(SuccessButton);
                return els.Count > 0 ? els[0].Text : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool IsLockoutMessageVisible()
        {
            try
            {
                var elements = _driver.FindElements(LockoutMessage);
                return elements.Count > 0 && elements[0].Displayed;
            }
            catch
            {
                return false;
            }
        }
        public void SignOut()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(SuccessButton)).Click();
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(SuccessButton));
        }

        // Sign out the current user, then sign in as another
        public void LoginAs(string username, string password)
        {
            SignOut();
            OpenSignIn();
            Login(username, password);
            WaitUntilLoggedIn();
        }
    }
}