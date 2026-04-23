// FILE: NavigationHelper.cs
// ROLE: Wraps driver navigation with base URL resolution.

using OpenQA.Selenium;

namespace Task1.Support
{
    public class NavigationHelper
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl;

        public NavigationHelper(IWebDriver driver, string baseUrl)
        {
            _driver  = driver;
            _baseUrl = baseUrl;
        }

        public void NavigateTo(string relativeUrl)
        {
            var target = _baseUrl.TrimEnd('/') + "/" + relativeUrl.TrimStart('/');
            _driver.Navigate().GoToUrl(target);
        }
    }
}
