# Task 1 — Mars Automation Framework

NUnit + Selenium WebDriver (C#) test automation for Project Mars.

---

## Current Progress
- ✅ Step 1: Smoke test — framework and login verified

## Coming Next
- Step 2: Language tests (Add / Update / Delete)
- Step 3: Skill tests (Add / Update / Delete)
- Step 4: Share Skill tests
- Step 5: Search Skill tests
- Step 6: Notification tests

---

## Setup

### 1. Prerequisites
- .NET 8 SDK
- Google Chrome (latest)
- Project Mars running at `http://localhost:5003`

### 2. Update credentials
Open `testsettings.json` and fill in your details:
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
    "Username": "your_email@example.com",
    "Password": "your_password"
  },
  "Report": {
    "Path": "Reports/TestReport.html",
    "Title": "Mars Task 1 - Test Report"
  }
}
```

### 3. Restore and build
```bash
dotnet restore
dotnet build
```

### 4. Run all tests
```bash
dotnet test
```

### 5. Run a specific test class
```bash
dotnet test --filter "ClassName=SmokeTest"
```

---

## Key URLs
| Page    | URL                                   |
|---------|---------------------------------------|
| Home    | http://localhost:5003                 |
| Profile | http://localhost:5003/Account/Profile |

---

## Folder Structure
```
Task1/
├── Assertions/        <- Assert helpers (one per feature)
├── Config/            <- TestSettings model
├── Hooks/             <- Base.cs — WebDriver, login, ExtentReports
├── Pages/             <- Page Object Models
├── Reports/           <- Generated HTML report (gitignored)
├── Screenshots/       <- Failure screenshots (gitignored)
├── Support/           <- Helpers (JSON reader, navigation, toasts, etc.)
├── TestData/          <- JSON test data files
├── TestDataModel/     <- Strongly-typed JSON model classes
├── Tests/             <- Test classes (one per feature)
├── testsettings.json  <- Runtime config
└── Task1.csproj       <- Project file + NuGet references
```

---

## Git Workflow
```bash
git add .
git commit -m "feat: add smoke test - framework and login verified"
git push origin main

```
---

## Test Report
After running tests, open:
```
Reports/TestReport.html
```
Screenshots for failed tests are saved to:
```
Screenshots/
```
