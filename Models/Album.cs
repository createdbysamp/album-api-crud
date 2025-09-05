namespace AlbumsApiCrud.Models;

public class Album
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public string? Artist { get; set; }
    public int ReleaseYear { get; set; }
    public string? Genre { get; set; }
    public string? AlbumTitle { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public override string ToString()
    {
        return $@"
AlbumId: {Id}
Album: {AlbumTitle}
ReleaseYear: {ReleaseYear:yyyy}
Artist: {Artist}
Genre: {Genre}
Rank: {Rank}";
    }
}
