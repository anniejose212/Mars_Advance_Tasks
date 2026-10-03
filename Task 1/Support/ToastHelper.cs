// FILE: ToastHelper.cs
// ROLE: Waits for and reads success/error toast notifications, and closes them.

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;


namespace Task1.Support
{
    public class ToastHelper
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators
       
        private static readonly By AnyToast =
            By.CssSelector("div.ns-box.ns-type-success .ns-box-inner, div.ns-box.ns-type-error .ns-box-inner");
        private static readonly By CloseButton =
            By.XPath("//a[@class='ns-close']");
        private static readonly By ToastBox =
            By.CssSelector("div.ns-box");

        public ToastHelper(IWebDriver driver, int timeoutSeconds = 10)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
        }

        // Reads whichever toast appears, success or error
        public string GetToastText()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(AnyToast)).Text.Trim();
        }


        // Closes any visible toast, then waits until it has gone so the next action starts with a clear screen
        public void CloseToastAndWait()
        {
            bool closed = false;

            foreach (var btn in _driver.FindElements(CloseButton))
            {
                if (btn.Displayed && btn.Enabled)
                {
                    btn.Click();
                    closed = true;
                }
            }

            if (closed)
                _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(ToastBox));
        }
    }
}
