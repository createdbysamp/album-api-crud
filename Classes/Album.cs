namespace AlbumsApiCrud.Classes;

public class Album
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public string? Artist { get; set; }
    public int ReleaseYear { get; set; }
    public string? Genre { get; set; }
    public string? AlbumTitle { get; set; }

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

    public static List<Album> GetAlbums()
    {
        // load album data from the JSON file on startup
        string filePath = "Data/albums.json";
        return Serializer.DeserializeFromFile<List<Album>>(filePath) ?? [];
        // returns deserialized data
    }
}
