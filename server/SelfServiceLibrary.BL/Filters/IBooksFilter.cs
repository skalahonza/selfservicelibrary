namespace SelfServiceLibrary.BL.Filters
{
    public interface IBooksFilter
    {
        string? Departmentnumber { get; }
        string? Name { get; }
        string? Author { get; }
        string? Status { get; }
        bool? IsAvailable { get; }
    }
}
