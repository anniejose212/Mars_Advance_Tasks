// FILE: DataCleanupHooks.cs
// ROLE: Cleanup helpers, called from each test class's BeforeEachTest / AfterEachTest,
//       so every test starts from a known state (Tip 4: test state matters).
//       Where a method takes "when", it is only for the log ("PRE-CLEAN" or "TEARDOWN").
//       Errors are logged, not thrown, so a cleanup problem never fails a test.

using NUnit.Framework;
using System;
using Task1.Pages.Components.Notifications;
using Task1.Pages.Components.Profile;
using Task1.Pages.Components.Profile.Overview;
using Task1.Pages.Components.ShareSkills;
using Task1.Support;
using Task1.TestDataModel;

namespace Task1.Hooks
{
    public static class DataCleanupHooks
    {
        // ===== ABOUT ME =====
        // Defaults come from AboutMeData.json, passed in by the test

        // Clicking the value closes the row when it's already at the default.
        // Otherwise select the default, then wait for the save toast before the next field.
        public static void ResetProfileToDefaults(AboutMe aboutMe, ToastHelper toasts, ProfileDefaults defaults)
        {
            TestContext.WriteLine("PRE-CLEAN(profile): ResetProfileToDefaults()");
            try
            {
                if (aboutMe.GetSelectedAvailability().Equals(defaults.Availability, StringComparison.OrdinalIgnoreCase))
                    aboutMe.ClickAvailabilityValue();
                else
                {
                    aboutMe.SelectAvailability(defaults.Availability);
                    toasts.GetToastText();
                    toasts.CloseToastAndWait();
                }

                if (aboutMe.GetSelectedHours().Equals(defaults.Hours, StringComparison.OrdinalIgnoreCase))
                    aboutMe.ClickHoursValue();
                else
                {
                    aboutMe.SelectHours(defaults.Hours);
                    toasts.GetToastText();
                    toasts.CloseToastAndWait();
                }

                if (aboutMe.GetSelectedEarnTarget().Equals(defaults.EarnTarget, StringComparison.OrdinalIgnoreCase))
                    aboutMe.ClickEarnTargetValue();
                else
                {
                    aboutMe.SelectEarnTarget(defaults.EarnTarget);
                    toasts.GetToastText();
                    toasts.CloseToastAndWait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[profile pre-clean] {ex.Message}");
            }
        }

        // Other suites search by seller name, so the name must always go back to the default
        public static void ResetNameToDefault(AboutMe aboutMe, ProfileDefaults defaults)
        {
            TestContext.WriteLine("RESET(profile): ResetNameToDefault()");
            try
            {
                string defaultName = defaults.FirstName + " " + defaults.LastName;
                if (aboutMe.GetDisplayedName() != defaultName)
                    aboutMe.UpdateName(defaults.FirstName, defaults.LastName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[name reset] {ex.Message}");
            }
        }

        // ===== LANGUAGES =====

        public static void CleanLanguages(LanguagesComponent languages, string when)
        {
            TestContext.WriteLine($"{when}(languages): DeleteAllLanguages()");
            try
            {
                languages.DeleteAllLanguages();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[languages {when}] {ex.Message}");
            }
        }

        // ===== SKILLS =====

        public static void CleanSkills(SkillsComponent skills, string when)
        {
            TestContext.WriteLine($"{when}(skills): DeleteAllSkills()");
            try
            {
                skills.DeleteAllSkills();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[skills {when}] {ex.Message}");
            }
        }

        // ===== SHARE SKILL LISTINGS =====
        // Call on the Manage Listings page

        public static void CleanListings(ShareSkillsComponent shareSkill, string when)
        {
            TestContext.WriteLine($"{when}(listings): DeleteAllListings()");
            try
            {
                shareSkill.DeleteAllListings();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[listings {when}] {ex.Message}");
            }
        }

        // ===== NOTIFICATIONS =====
        // Marks everything as read so each test starts with no unread badge

        public static void CleanNotifications(NotificationDropdownComponent notifications, string when)
        {
            TestContext.WriteLine($"{when}(notifications): MarkAllAsReadIfAny()");
            try
            {
                notifications.MarkAllAsReadIfAny();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[notifications {when}] {ex.Message}");
            }
        }

        // ===== SENT REQUESTS (second user) =====
        // A pending request blocks the owner from deleting the listing, so B withdraws first.

        public static void CleanSentRequests(ManageRequestsComponent request)
        {
            TestContext.WriteLine("CLEAN(sent requests): WithdrawAllRequests()");
            try
            {
                request.WithdrawAllRequests();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[sent requests] {ex}");
            }
        }
    }
}
