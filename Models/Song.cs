namespace SpotifyClone.Models;

public class Song
{
    public int Id { get; set; }
    public string VideoId { get; set; }  // YouTube video ID
    public string Title { get; set; }
    public int Duration { get; set; }    // in seconds
    public List<PlaylistSong> PlaylistSongs { get; set; } = new();
}
