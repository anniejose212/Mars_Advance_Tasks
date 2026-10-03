// FILE: SkillsModel.cs
// ROLE: Test data model for SkillsData.json. One section per scenario type.

using System.Collections.Generic;

namespace Task1.TestDataModel
{
    public class SkillsModel
    {
        public SkillEntry Add { get; set; }
        public SkillUpdateEntry Update { get; set; }
        public SkillEntry Delete { get; set; }
        public List<SkillEntry> MultipleSkills { get; set; }
        public SkillDuplicateEntry Duplicate { get; set; }
        public SkillDuplicateEntry CaseInsensitiveDuplicate { get; set; }
        public SkillEntry UnsafeInput { get; set; }
        public int MassiveNameLength { get; set; }

        // ExpectedToastText is defined in LanguagesModel.cs (same namespace)
        public ExpectedToastText ExpectedToasts { get; set; }
    }

    public class SkillEntry
    {
        public string Skill { get; set; }
        public string Level { get; set; }
    }

    public class SkillUpdateEntry
    {
        public string Skill { get; set; }
        public string Level { get; set; }
        public string NewLevel { get; set; }
    }

    public class SkillDuplicateEntry
    {
        public string ExistingSkill { get; set; }
        public string DuplicateAttempt { get; set; }
        public string Level { get; set; }
    }
}
