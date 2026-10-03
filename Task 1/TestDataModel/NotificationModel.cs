// FILE: NotificationModel.cs
// ROLE: Maps NotificationData.json. Reuses ShareSkillListing for the listing B requests.

namespace Task1.TestDataModel
{
    public class NotificationModel
    {
        // A's listing. Skill-exchange, so the "You might not have the skills" Yes/No popup appears
        public ShareSkillListing Listing { get; set; }

        // B's request
        public string RequestMessage { get; set; }

        // What A should see
        public string SenderName { get; set; }
        public string NotificationText { get; set; }
        public string WithdrawText { get; set; }

        // Expected messages
        public string RequestSentToast { get; set; }
        public string EmptyMessage { get; set; }
    }
}
