namespace SharingCsm.Library.AppHost;

public abstract class ResourceNames
{
    private const string LibraryPrefix = "library-";
    
    public const string Postgres = LibraryPrefix + "postgres";
    public const string LibraDataVolume = LibraryPrefix + "data-volume";
    public const string LibraryDb = "LibraryDb";
    public const string MigrationsService = LibraryPrefix + "migrations";
    public const string Api = LibraryPrefix + "api";
}