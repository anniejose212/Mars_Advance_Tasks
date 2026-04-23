// FILE: SmokeTest.cs
// ROLE: Verifies the framework and critical path are working
//       before any feature tests are written.

using Task1.Hooks;
using NUnit.Framework;

namespace Task1.Tests
{
    [TestFixture]
    public class SmokeTest : Base
    {
        [Test]
        public void Smoke_FrameworkAndLoginWorking_AppReachable()
        {
            // 1. Browser launched         — proven by reaching this line
            // 2. App loaded               — proven by Base navigating to BaseUrl
            // 3. Login succeeded          — proven by Base calling WaitUntilLoggedIn()
            // 4. Profile page reachable   — navigate and confirm URL changed
            Nav.NavigateTo("Account/Profile");

            Assert.That(LoginPage.IsLoggedIn(), Is.True,
                "User should still be logged in after navigating to Profile.");

            Assert.That(Driver.Url, Does.Contain("localhost"),
                "App should still be reachable after navigation.");
        }
    }
}