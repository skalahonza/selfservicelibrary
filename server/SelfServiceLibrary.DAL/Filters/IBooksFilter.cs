namespace SelfServiceLibrary.DAL.Filters
{
    public interface IBooksFilter
    {
        string? Departmentnumber { get; }
        string? Name { get; }
        string? Author { get; }
        string? Status { get; }
        string? PublicationType { get; }
        bool? IsAvailable { get; }
        bool? IsVisible { get; }
    }
}
