// FILE: ToastHelper.cs
// ROLE: Waits for and reads success/error toast notifications.

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Task1.Support
{
    public class ToastHelper
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private static readonly By ErrorToast =
            By.CssSelector("div.ns-box.ns-type-error .ns-box-inner");

        private static readonly By SuccessToast =
            By.CssSelector("div.ns-box.ns-type-success .ns-box-inner");

        private readonly By CloseButton =
            By.XPath("//a[@class='ns-close']");

        public ToastHelper(IWebDriver driver, int timeoutSeconds = 10)
        {
            _driver = driver;
            _wait   = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
        }

        public string GetSuccessToastText()
        {
            return _wait.Until(d => d.FindElement(SuccessToast)).Text.Trim();
        }

        public string GetErrorToastText()
        {
            return _wait.Until(d => d.FindElement(ErrorToast)).Text.Trim();
        }

        public void CloseToast()
        {
            foreach (var btn in _driver.FindElements(CloseButton))
                if (btn.Displayed && btn.Enabled) btn.Click();
        }
    }
}
