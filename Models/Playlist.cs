namespace SpotifyClone.Models;

public class Playlist
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<PlaylistSong> PlaylistSongs { get; set; } = new();
}
