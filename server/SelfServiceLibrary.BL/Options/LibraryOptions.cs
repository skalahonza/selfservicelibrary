namespace SelfServiceLibrary.BL.Options
{
    public class LibraryOptions
    {
        /// <summary>
        /// How many days before issue expires should the users be reminded. Multiple days can be set.
        /// </summary>
        public int[]? IssueReminderDaysBefore { get; set; }
    }
}
