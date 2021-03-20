namespace SelfServiceLibrary.Persistence.Entities
{
    public class BookStatus
    {
        public string Name { get; set; } = "Default";

        public bool IsVissible { get; set; } = true;

        public bool CanBeBorrowed { get; set; } = true;
    }
}
