// FILE: LoginSteps.cs
// ROLE: Login steps shared by every feature (used in Background).

using OpenQA.Selenium;
using Reqnroll;
using Task2.Config;
using Task2.Pages;
using Task2.Support;

namespace Task2.Steps
{
    [Binding]
    public class LoginSteps
    {
        private readonly LoginPage _loginPage;
        private readonly TestSettings _settings;

        public LoginSteps(IWebDriver driver, TestSettings settings)
        {
            _settings = settings;
            var nav = new NavigationHelper(driver, settings.Environment.BaseUrl);
            _loginPage = new LoginPage(driver, nav);
        }

        [Given("user A is logged in")]
        public void GivenUserAIsLoggedIn()
        {
            _loginPage.OpenSignIn();
            _loginPage.Login(_settings.Login.Username, _settings.Login.Password);
            _loginPage.WaitUntilLoggedIn();
        }
    }
}