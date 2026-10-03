# Mars Advanced Task, Part 1: Automation Framework

NUnit + Selenium WebDriver (C#) automation for Project Mars, covering the Column A features (A2 to A16) from the Advanced Task brief.

Built with a component-based Page Object Model, JSON test data, ExtentReports and cleanup hooks so every test starts from a known state.

---

## Coverage

| Area | Test class | Tests | Column A |
|------|-----------|-------|----------|
| Profile: Availability, Hours, Earn Target, Name, Location | `AboutMeTests` | 6 | A2 |
| Languages: Add, Update, Delete | `LanguagesTests` | 13 | A3 to A5 |
| Skills: Add, Update, Delete | `SkillsTests` | 11 | A6 to A8 |
| Share Skill: Add New | `ShareSkillsTests` | 14 | A9 |
| Search Skills: Categories, Sub-categories, Filters | `SearchSkillsTests` | 20 | A10, A11 |
| Notifications: Dropdown and Dashboard | `NotificationTests` | 17 | A12 to A16 |

Each area includes happy path, extended positive, negative (valid and invalid input), security and destructive tests, as described in the brief.

Manual test cases for User Story 1 are in `Mars_Task_4_Part_1_Manual_Test_Cases.xlsx` (81 cases, with actual results), including untestable scenarios and known issues with reasons.

**Latest full run:** 78 passed, 0 failed, 3 skipped (known app defects, see below).

---

## Setup

### 1. Prerequisites
- .NET 8 SDK
- Google Chrome (latest)
- Project Mars running at `http://localhost:5003`
- Two Project Mars accounts (the Notification tests need a second user to send trade requests)

### 2. Update credentials
Open `testsettings.json` and fill in both accounts:
```json
{
  "Browser": {
    "Type": "chrome",
    "Headless": false,
    "TimeoutSeconds": 10
  },
  "Environment": {
    "BaseUrl": "http://localhost:5003"
  },
  "Login": {
    "Username": "user_a@example.com",
    "Password": "your_password"
  },
  "SecondLogin": {
    "Username": "user_b@example.com",
    "Password": "your_password"
  },
  "Report": {
    "Path": "Reports/TestReport.html",
    "Title": "Mars Task 1 - Test Report"
  }
}
```
- `Login` (user A) owns listings and receives notifications.
- `SecondLogin` (user B) searches for A's listing and sends or withdraws trade requests.

### 3. Restore and build
```bash
dotnet restore
dotnet build
```

---

## Running tests

Run everything:
```bash
dotnet test
```

Run the smoke set first on every build (one happy path per area):
```bash
dotnet test --filter Category=Smoke
```

Run one area:
```bash
dotnet test --filter Category=Profile_Languages
```

| Category | What it runs |
|----------|--------------|
| `Smoke` | One happy path per area (8 tests) |
| `Profile_Settings` | About Me tests |
| `Profile_Languages` | Languages tests |
| `Profile_Skills` | Skills tests |
| `Share_Skill` | Share Skill tests |
| `Search_Skills` | Search Skills tests |
| `Notifications` | Notification dropdown and Dashboard tests |
| `Security` | Unsafe input tests (script tags, SQL injection) |
| `Destructive` | Very large input tests |
| `KnownIssue` | Tests linked to a known app defect (skipped or recording current behaviour) |

In Visual Studio Test Explorer, choose Group By > Traits to see these categories.

---

## Framework structure

```
Task1/
├── Assertions/          <- One assertion class per feature (no asserts in pages)
├── Config/              <- TestSettings model for testsettings.json
├── Hooks/
│   ├── Base.cs              <- Browser, login, ExtentReports, screenshots
│   └── DataCleanupHooks.cs  <- Pre-clean and teardown for each feature
├── Pages/
│   ├── LoginPage.cs
│   └── Components/      <- Mirrors the app's UI
│       ├── Notifications/   <- Notification dropdown, Dashboard, Manage Requests
│       ├── Profile/         <- About Me
│       │   └── Overview/    <- Languages, Skills
│       ├── SearchSkills/
│       └── ShareSkills/
├── Support/             <- DriverFactory, JsonFileReader, TestDataPath, NavigationHelper,
│                           ToastHelper, AlertHelpers, UiTextHelper
├── TestData/            <- JSON test data, one file per feature
├── TestDataModel/       <- Classes that bind each JSON file
├── Tests/               <- One test class per feature
└── testsettings.json    <- Runtime config
```

Flow: **Tests** call **Components** to act on the page, read data from **TestData** through **TestDataModel**, and check results with **Assertions**. **Hooks** set up and clean up around every test.

### Key design points
- **Component POM:** pages are split into components that match what the user sees (for example Profile > Overview > Languages).
- **JSON test data:** all inputs and expected messages come from JSON, read at test level.
- **Test state:** cleanup hooks delete languages, skills and listings, withdraw sent requests and mark notifications as read before and after each test.
- **Single driver setup:** `DriverFactory` creates the browser in one place.
- **Reporting:** each test's log lines (setup steps, toast text, expected vs actual) and a screenshot on failure go into the Extent report.
- **Shared helpers:** toasts are read and closed through `ToastHelper`; table text is compared through `UiTextHelper`.

---

## Known issues

These are app defects. The tests that cover them are marked `[Ignore]` with the reason, so they are skipped until the app is fixed. Remove the `[Ignore]` line to run them again. They are also logged on the Known Issues sheet of the workbook.

| Test | Issue |
|------|-------|
| `NTF_TC_015` | Dashboard Show Less leaves (total minus 5) rows instead of 5 |
| `LNG_TC_012`, `SKL_TC_007` | A script tag is saved as a language or skill name |

Recorded as current behaviour (tests pass):
- `LNG_TC_007`, `SKL_TC_006`: duplicates that differ only in case are accepted as a new entry. If the app is fixed, change both tests to expect the duplicate to be rejected.

Not automated:
- Share Skill availability scheduler: the event popup Save does not save, even manually.
- Work sample upload: opens the operating system file dialog.

---

## Test report

After a run, open:
```
Reports/TestReport.html
```
Screenshots of failed tests are saved to:
```
Screenshots/
```

---

## Git workflow

Work on a feature branch and keep `main` clean:
```bash
git checkout -b feature/part1-automation
git add .
git commit -m "Add Part 1 automation suite"
git push origin feature/part1-automation
```
