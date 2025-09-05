using Microsoft.EntityFrameworkCore;

namespace AlbumsApiCrud.Models;

public class AlbumContext : DbContext
{
    // DbSet<T> properties represent our tables
    public DbSet<Album> ALbums { get; set; }

    // the constructor will be configured in Program.cs
    public AlbumContext(DbContextOptions<AlbumContext> options)
        : base(options) { }
}
