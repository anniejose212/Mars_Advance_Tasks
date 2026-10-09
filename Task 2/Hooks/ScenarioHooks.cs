// FILE: ScenarioHooks.cs
// ROLE: Reqnroll hooks. Sets up the Extent report once per run, starts a browser per
//       scenario and shares it with step classes, logs every step to the report with a
//       screenshot on failure, and closes the browser after each scenario.

using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using OpenQA.Selenium;
using Reqnroll;
using Reqnroll.BoDi;
using System.Net;
using Task2.Config;
using Task2.Support;

namespace Task2.Hooks
{
    [Binding]
    public class ScenarioHooks
    {
        private static ExtentReports? _extent;
        private static TestSettings? _settings;

        private readonly IObjectContainer _container;
        private readonly ScenarioContext _scenarioContext;
        private IWebDriver? _driver;
        private ExtentTest? _test;

        public ScenarioHooks(IObjectContainer container, ScenarioContext scenarioContext)
        {
            _container = container;
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _settings = JsonFileReader.Load<TestSettings>("testsettings.json");

            string reportPath = Path.Combine(ProjectRoot(), _settings.Report.Path);
            Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

            var sparkReporter = new ExtentSparkReporter(reportPath);
            sparkReporter.Config.DocumentTitle = _settings.Report.Title;
            sparkReporter.Config.ReportName = _settings.Report.Title;

            _extent = new ExtentReports();
            _extent.AttachReporter(sparkReporter);
            Console.WriteLine($"[INFO] Report path: {reportPath}");
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _test = _extent!.CreateTest(HtmlSafe(_scenarioContext.ScenarioInfo.Title));

            _driver = DriverFactory.Create(_settings!.Browser.Type, _settings.Browser.Headless);
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;

            _container.RegisterInstanceAs<IWebDriver>(_driver);
            _container.RegisterInstanceAs<TestSettings>(_settings);
        }

        [AfterStep]
        public void AfterStep()
        {
            var step = _scenarioContext.StepContext.StepInfo;
            string stepText = HtmlSafe(step.StepDefinitionType + " " + step.Text);

            if (_scenarioContext.TestError == null)
            {
                _test!.Pass(stepText);
                return;
            }

            _test!.Fail(stepText + "<br>" + HtmlSafe(_scenarioContext.TestError.Message));
            string screenshotPath = SaveScreenshot(_scenarioContext.ScenarioInfo.Title);
            if (screenshotPath != "")
                _test.AddScreenCaptureFromPath(screenshotPath);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            try
            {
                _driver?.TryDismissAnyAlert();
            }
            finally
            {
                _driver?.Quit();
                _driver?.Dispose();
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _extent?.Flush();
            Console.WriteLine("[INFO] Report flushed.");
        }

        private string SaveScreenshot(string scenarioName)
        {
            try
            {
                string folder = Path.Combine(ProjectRoot(), "Screenshots");
                Directory.CreateDirectory(folder);

                string file = Path.Combine(folder,
                    $"{MakeSafeFileName(scenarioName)}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

                var shot = ((ITakesScreenshot)_driver!).GetScreenshot();
                File.WriteAllBytes(file, shot.AsByteArray);
                return file;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Screenshot failed: {ex.Message}");
                return "";
            }
        }

        private static string MakeSafeFileName(string name)
        {
            string safe = name;
            foreach (char c in Path.GetInvalidFileNameChars())
                safe = safe.Replace(c, '_');
            return safe == "" ? "Scenario" : safe;
        }

        private static string ProjectRoot()
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        }

        private static string HtmlSafe(string text)
        {
            return WebUtility.HtmlEncode(text ?? "");
        }
    }
}