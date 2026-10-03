// FILE: ShareSkillModel.cs
// ROLE: Maps ShareSkillData.json. One property per test scenario.

using System.Collections.Generic;

namespace Task1.TestDataModel
{
    public class ShareSkillModel
    {
        // Positive
        public ShareSkillListing ValidHourlySkillExchange { get; set; }
        public ShareSkillListing ValidOneOffCredit { get; set; }

        // Negative
        public ShareSkillListing EmptyForm { get; set; }
        public ShareSkillListing EmptyTitle { get; set; }
        public ShareSkillListing EmptyDescription { get; set; }
        public ShareSkillListing NoTags { get; set; }
        public ShareSkillListing SpecialCharsTitle { get; set; }
        public ShareSkillListing SpecialCharsDescription { get; set; }

        // Security
        public ShareSkillListing XssTitle { get; set; }
        public ShareSkillListing SqlInjectionDescription { get; set; }

        // Boundary
        public BoundaryInput TitleOverMax { get; set; }
        public BoundaryInput DescriptionOverMax { get; set; }
        public BoundaryInput CreditOverMax { get; set; }

        // Expected messages
        public string ErrorToast { get; set; }
    }

    // One Share Skill form submission. Empty strings and empty lists are skipped when filling.
    public class ShareSkillListing
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public List<string> Tags { get; set; }
        public string ServiceType { get; set; }          // "Hourly" or "One-off"
        public string LocationType { get; set; }         // "On-site" or "Online"
        public string SkillTrade { get; set; }           // "Skill-exchange" or "Credit"
        public List<string> SkillExchangeTags { get; set; }
        public string Credit { get; set; }               // 0 to 10
        public string Active { get; set; }               // "Active" or "Hidden"
    }

    // Limit check on a single field
    public class BoundaryInput
    {
        public string Input { get; set; }
        public int ExpectedLength { get; set; }
    }
}
