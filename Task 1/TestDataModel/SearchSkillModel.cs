// FILE: SearchSkillModel.cs
// ROLE: Maps SearchSkillData.json. Reuses ShareSkillListing to create the listings searched for.

namespace Task1.TestDataModel
{
    public class SearchSkillModel
    {
        // Listings created in setup (Listing) and in the hidden test (HiddenListing)
        public ShareSkillListing Listing { get; set; }
        public ShareSkillListing HiddenListing { get; set; }

        // Search text
        public string PartialTitle { get; set; }
        public string DifferentCaseTitle { get; set; }
        public string TrimmedTitle { get; set; }
        public string NoMatchText { get; set; }
        public string SpecialCharsText { get; set; }
        public string UnsafeText { get; set; }
        public int HugePayloadLength { get; set; }

        // User search: the logged-in user who owns Listing
        public string SellerName { get; set; }

        // Filters
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string OtherCategory { get; set; }
        public string OnlineFilter { get; set; }
        public string OnsiteFilter { get; set; }
        public string ShowAllFilter { get; set; }

        // Expected values
        public string ExpectedCategoryCount { get; set; }
        public string NoResultsText { get; set; }
    }
}