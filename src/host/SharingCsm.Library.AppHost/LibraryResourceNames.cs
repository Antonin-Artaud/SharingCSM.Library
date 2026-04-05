namespace SharingCsm.Library.AppHost;

public abstract class LibraryResourceNames
{
    private const string LibraryPrefix = "library-";
    
    public const string Postgres = LibraryPrefix + "postgres";
    public const string PostgresDataVolume = LibraryPrefix + "data-volume";
    public const string Database = LibraryPrefix + "database";
    public const string MigrationsService = LibraryPrefix + "migrations";
    public const string Api = LibraryPrefix + "api";
}