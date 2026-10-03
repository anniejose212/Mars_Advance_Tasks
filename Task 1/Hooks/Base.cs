// FILE: Base.cs
// ROLE: Base class for every test fixture. Starts the browser, logs in, creates the
//       Extent report node, copies the test's log lines into the report, and saves a
//       screenshot when a test fails.
//       Test classes add their own setup and cleanup by overriding BeforeEachTest / AfterEachTest.

using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using System;
using System.IO;
using System.Net;
using Task1.Config;
using Task1.Pages;
using Task1.Pages.Components.Profile;
using Task1.Support;

namespace Task1.Hooks
{
    [TestFixture]
    public abstract class Base
    {
        // =====================================================================
        // CORE
        // =====================================================================
        protected IWebDriver Driver;

        protected static ExtentReports Extent;
        private static readonly object _extentLock = new object();

        [ThreadStatic]
        protected static ExtentTest Test;

        protected TestSettings Settings;

        // =====================================================================
        // HELPERS AND PAGES
        // =====================================================================
        protected NavigationHelper Nav;
        protected ToastHelper Toasts;

        protected LoginPage LoginPage;
        protected AboutMe AboutMePage;

        private static string HtmlSafe(string s) =>
            WebUtility.HtmlEncode(s ?? string.Empty);

        // =====================================================================
        // GLOBAL ONE-TIME SETUP
        // =====================================================================
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            Settings = JsonFileReader.Load<TestSettings>("testsettings.json");

            lock (_extentLock)
            {
                if (Extent != null) return;

                var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
                var reportPath = Path.Combine(root, Settings.Report.Path);
                var reportDir = Path.GetDirectoryName(reportPath);

                if (!string.IsNullOrEmpty(reportDir))
                    Directory.CreateDirectory(reportDir);

                try
                {
                    var htmlReporter = new ExtentSparkReporter(reportPath);
                    htmlReporter.Config.DocumentTitle = Settings.Report.Title;
                    htmlReporter.Config.ReportName = Settings.Report.Title;

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
        }

        // =====================================================================
        // PER-TEST SETUP
        // =====================================================================
        [SetUp]
        public void TestSetup()
        {
            // Create the report node first, so a failure in setup is still reported
            lock (_extentLock)
            {
                Test = Extent.CreateTest(HtmlSafe(TestContext.CurrentContext.Test.Name));
            }

            Driver = DriverFactory.Create(Settings.Browser.Type, Settings.Browser.Headless);
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;

            Nav = new NavigationHelper(Driver, Settings.Environment.BaseUrl);
            Toasts = new ToastHelper(Driver);

            LoginPage = new LoginPage(Driver, Nav);
            AboutMePage = new AboutMe(Driver);

            LoginPage.OpenSignIn();
            LoginPage.Login(Settings.Login.Username, Settings.Login.Password);
            LoginPage.WaitUntilLoggedIn();

            BeforeEachTest();
        }

        // Test classes override these for their own setup and cleanup
        protected virtual void BeforeEachTest() { }
        protected virtual void AfterEachTest() { }

        // =====================================================================
        // PER-TEST TEARDOWN
        // =====================================================================
        [TearDown]
        public void TestTeardown()
        {
            var result = TestContext.CurrentContext.Result;
            var status = result.Outcome.Status;
            var message = result.Message;

            // Driver is null if the browser never started
            Driver?.TryDismissAnyAlert();

            try
            {
                WriteTestOutputToReport();

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
        // REPORT LOGGING
        // =====================================================================

        // Copies everything the test wrote with TestContext.WriteLine (setup, steps,
        // assertion messages) into the Extent report, one line per entry
        private void WriteTestOutputToReport()
        {
            string output = TestExecutionContext.CurrentContext.CurrentResult.Output;
            if (string.IsNullOrWhiteSpace(output)) return;

            foreach (var line in output.Split('\n'))
            {
                if (line.Trim() != "")
                    Test.Info(HtmlSafe(line.Trim()));
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
                var dir = Path.Combine(root, "Screenshots");
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
                Console.WriteLine("[INFO] Report flushed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Flush failed: {ex.Message}");
            }
        }
    }
}