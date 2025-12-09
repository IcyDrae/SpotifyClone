using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Models;
using Microsoft.EntityFrameworkCore;

namespace SpotifyClone.Controllers;

public class PlaylistController : Controller
{
    private readonly AppDbContext _context;

    public PlaylistController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Playlist playlist)
    {
        if (string.IsNullOrWhiteSpace(playlist.Name))
            return BadRequest("Playlist name cannot be empty.");

        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();
        return Ok(playlist);
    }

    [HttpPost("/playlist/add")]
    public async Task<IActionResult> AddToPlaylist([FromBody] AddToPlaylistDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.VideoId))
        return BadRequest("Invalid video data.");

        // 1. Add the Song (if not exists)
        var song = await _context.Songs
            .FirstOrDefaultAsync(s => s.VideoId == dto.VideoId);

        if (song == null)
        {
            song = new Song
            {
                VideoId = dto.VideoId,
                Title = dto.Title,
                Duration = int.TryParse(dto.Duration.Split(':')[0], out int min) ? min * 60 + int.Parse(dto.Duration.Split(':')[1]) : 0
            };
            _context.Songs.Add(song);
            await _context.SaveChangesAsync();
        }

        // 2. Add to PlaylistSong if not exists
        var exists = await _context.PlaylistSongs
            .AnyAsync(ps => ps.PlaylistId == dto.PlaylistId && ps.SongId == song.Id);

        if (!exists)
        {
            _context.PlaylistSongs.Add(new PlaylistSong
            {
                PlaylistId = dto.PlaylistId,
                SongId = song.Id
            });
            await _context.SaveChangesAsync();
        }

        return Ok();
    }

    public async Task<IActionResult> ViewPlaylist(int id)
    {
        // Load playlist with songs
        var playlist = await _context.Playlists
            .Include(p => p.PlaylistSongs)
            .ThenInclude(ps => ps.Song)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (playlist == null)
            return NotFound();

        // Load all playlists for sidebar
        var allPlaylists = await _context.Playlists.ToListAsync();
        ViewData["Playlists"] = allPlaylists;

        return View(playlist);
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }
}

// DTO class
public class AddToPlaylistDto
{
    public int PlaylistId { get; set; }
    public string VideoId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Duration { get; set; } = null!;
}

