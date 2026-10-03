namespace Task1.TestDataModel
{
    public class AboutMeModel

    {
        public ProfileDefaults Defaults { get; set; }
        public string Availability { get; set; }
        public string Hours { get; set; }
        public string EarnTarget { get; set; }

        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }

        public string SuccessToast { get; set; }
    }
    public class ProfileDefaults
    {
        public string Availability { get; set; }
        public string Hours { get; set; }
        public string EarnTarget { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}