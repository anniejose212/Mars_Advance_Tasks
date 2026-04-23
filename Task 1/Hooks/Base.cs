// FILE: Base.cs
// ROLE: Base test fixture — WebDriver lifecycle, login, ExtentReports, screenshots.
//       Start: login only. More page objects will be added in later pushes.

using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Task1.Config;
using Task1.Pages;
using Task1.Support;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;
using System.Net;

namespace Task1.Hooks
{
    [TestFixture]
    public class Base : LoggerHelper
    {
        // ── Core infrastructure ───────────────────────────────────────────────
        protected IWebDriver Driver;
        protected static ExtentReports Extent;
        protected static ExtentTest Test;
        protected TestSettings Settings;

        // ── Helpers ───────────────────────────────────────────────────────────
        protected NavigationHelper Nav;
        protected ToastHelper Toasts;

        // ── Pages ─────────────────────────────────────────────────────────────
        protected LoginPage LoginPage;

        private static string HtmlSafe(string s) =>
            WebUtility.HtmlEncode(s ?? string.Empty);

        // =====================================================================
        // GLOBAL ONE-TIME SETUP
        // =====================================================================
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            Settings = JsonFileReader.Load<TestSettings>("testsettings.json");

            // Resolve report path relative to project root
            var root       = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            var reportPath = Path.Combine(root, Settings.Report.Path);
            var reportDir  = Path.GetDirectoryName(reportPath);

            if (!string.IsNullOrEmpty(reportDir))
                Directory.CreateDirectory(reportDir);

            try
            {
                var htmlReporter = new ExtentSparkReporter(reportPath);
                htmlReporter.Config.DocumentTitle = Settings.Report.Title;
                htmlReporter.Config.ReportName    = Settings.Report.Title;

                Extent = new ExtentReports();
                Extent.AttachReporter(htmlReporter);

                Console.WriteLine($"[INFO] Report path: {reportPath}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Extent setup failed: {e}");
                throw;
            }
        }

        // =====================================================================
        // PER-TEST SETUP
        // =====================================================================
        [SetUp]
        public void TestSetup()
        {
            Driver = CreateWebDriver();
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait =
                TimeSpan.FromSeconds(Settings.Browser.TimeoutSeconds);

            // ── Helpers & pages ───────────────────────────────────────────────
            Nav       = new NavigationHelper(Driver, Settings.Environment.BaseUrl);
            Toasts    = new ToastHelper(Driver);
            LoginPage = new LoginPage(Driver, Nav);

            // ── Login ─────────────────────────────────────────────────────────
            LoginPage.OpenSignIn();
            LoginPage.Login(Settings.Login.Username, Settings.Login.Password);
            LoginPage.WaitUntilLoggedIn();

            // ── Optional per-test preconditions ──────────────────────────────
            BeforeEachTest();

            // ── Extent node for this test ─────────────────────────────────────
            Test = Extent.CreateTest(HtmlSafe(TestContext.CurrentContext.Test.Name));
        }

        // ── Browser factory ───────────────────────────────────────────────────
        private IWebDriver CreateWebDriver()
        {
            var type = Settings.Browser.Type?.ToLowerInvariant() ?? "chrome";

            if (type == "firefox")
            {
                var opts = new FirefoxOptions();
                if (Settings.Browser.Headless) opts.AddArgument("--headless");
                return new FirefoxDriver(opts);
            }

            var ch = new ChromeOptions();
            if (Settings.Browser.Headless) ch.AddArgument("--headless=new");
            return new ChromeDriver(ch);
        }

        // ── Virtual hooks for derived test classes ────────────────────────────
        protected virtual void BeforeEachTest() { }
        protected virtual void AfterEachTest()  { }

        // =====================================================================
        // PER-TEST TEARDOWN
        // =====================================================================
        [TearDown]
        public void TestTeardown()
        {
            var result  = TestContext.CurrentContext.Result;
            var status  = result.Outcome.Status;
            var message = result.Message;

            Driver.TryDismissAnyAlert();

            try
            {
                foreach (var line in GetLogs())
                    Test.Info(HtmlSafe(line));
                ClearLogs();

                if (status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    var path = SaveScreenshot(TestContext.CurrentContext.Test.Name);
                    if (!string.IsNullOrEmpty(path))
                        Test.Fail(HtmlSafe(message)).AddScreenCaptureFromPath(path);
                    else
                        Test.Fail(HtmlSafe(message));
                }
                else
                {
                    Test.Pass("Passed");
                    TestContext.WriteLine("Test passed");
                }

                AfterEachTest();
            }
            finally
            {
                Driver?.Quit();
                Driver?.Dispose();
            }
        }

        // =====================================================================
        // SCREENSHOT
        // =====================================================================
        protected string SaveScreenshot(string testName)
        {
            try
            {
                var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
                var dir  = Path.Combine(root, "Screenshots");
                Directory.CreateDirectory(dir);

                var file = Path.Combine(dir,
                    $"{MakeSafeFileName(testName)}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

                var shot = ((ITakesScreenshot)Driver).GetScreenshot();
                File.WriteAllBytes(file, shot.AsByteArray);
                Console.WriteLine($"[SCREENSHOT] {file}");
                return file;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Screenshot failed: {ex.Message}");
                return string.Empty;
            }
        }

        private string MakeSafeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Test";
            var safe = name;
            foreach (var c in Path.GetInvalidFileNameChars())
                safe = safe.Replace(c, '_');
            safe = safe.Replace("\"", "").Replace("(", "_").Replace(")", "_");
            return string.IsNullOrWhiteSpace(safe) ? "Test" : safe;
        }

        // =====================================================================
        // GLOBAL TEARDOWN
        // =====================================================================
        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            try
            {
                Extent?.Flush();
                Console.WriteLine($"[INFO] Report flushed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Flush failed: {ex.Message}");
            }
        }
    }
}
