// FILE: LanguagesModel.cs
// ROLE: Test data model for LanguagesData.json. One section per scenario type.

using System.Collections.Generic;

namespace Task1.TestDataModel
{
    public class LanguagesModel
    {
        public LanguageEntry Add { get; set; }
        public LanguageUpdateEntry Update { get; set; }
        public LanguageEntry Delete { get; set; }
        public List<LanguageEntry> MultipleLanguages { get; set; }
        public List<LanguageEntry> FourLanguages { get; set; }
        public LanguageDuplicateEntry Duplicate { get; set; }
        public LanguageDuplicateEntry CaseInsensitiveDuplicate { get; set; }
        public LanguageInvalidLevelEntry InvalidLevel { get; set; }
        public LanguageEntry UnsafeInput { get; set; }
        public int MassiveNameLength { get; set; }
        public ExpectedToastText ExpectedToasts { get; set; }
    }

    public class LanguageEntry
    {
        public string Language { get; set; }
        public string Level { get; set; }
    }

    public class LanguageUpdateEntry
    {
        public string Language { get; set; }
        public string Level { get; set; }
        public string NewLevel { get; set; }
    }

    public class LanguageDuplicateEntry
    {
        public string ExistingLanguage { get; set; }
        public string DuplicateAttempt { get; set; }
        public string Level { get; set; }
    }

    public class LanguageInvalidLevelEntry
    {
        public string Language { get; set; }
        public string InvalidLevel { get; set; }
    }

    // Words the toast must contain, so expected text lives in JSON, not in the tests
    public class ExpectedToastText
    {
        public string Added { get; set; }
        public string Updated { get; set; }
        public string Deleted { get; set; }
        public string Duplicate { get; set; }
        public string MissingInput { get; set; }
    }
}