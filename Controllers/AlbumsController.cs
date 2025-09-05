using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AlbumsApiCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlbumsApiCrud.Controllers;

// general wholesale controller

[ApiController]
[Route("api/albums")]
public class AlbumsController : ControllerBase
{
    // created mutable in-memory list
    // private static List<Album> _albums = Album.GetAlbums();

    private readonly AlbumContext _context;

    // constructor
    public AlbumsController(AlbumContext context)
    {
        _context = context;
    }

    // READ ENDPOINTS (GET)

    // 1. GET ALL ALBUMS
    [HttpGet]
    public async Task<ActionResult<List<Album>>> GetAllAlbums()
    {
        // set up album variable w/i method

        var _albums = await _context.Albums.ToListAsync();
        if (_albums.Count == 0)
        {
            return NotFound("No albums found. Something is wrong."); // returns 404 NOT FOUND status code
        }
        return Ok(_albums); // 200 OK status code with the album list
    }

    // 2. GET A SINGLE ALBUM BY ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Album>> GetAlbumById(int id)
    {
        // use LINQ to find the album with the matching ID
        var searchedAlbum = await _context.Albums.FirstOrDefaultAsync(album => album.Id == id);

        if (searchedAlbum is null)
        {
            return NotFound("Specified album not found.."); // 404 not found
        }

        return Ok(searchedAlbum); // returns 200 OK with the album object
    }

    // CREATE ENDPOINT (POST)

    // 1. CREATE A NEW ALBUM

    [HttpPost]
    public async Task<ActionResult<Album>> CreateAlbum(Album newAlbum)
    {
        // auto generate new id
        // var nextId = _albums.Count != 0 ? _albums.Max(a => a.Id) + 1 : 1;
        // newAlbum.Id = nextId;

        // add new album to in-memory list
        _context.Albums.Add(newAlbum);

        // save changes
        await _context.SaveChangesAsync(); // commits changes to db

        // return a success response
        // CreatedAtAction helper method returns a 201 Created status code
        return CreatedAtAction(nameof(GetAlbumById), new { id = newAlbum.Id }, newAlbum);
    }

    // UPDATE ENDPOINT (PUT)

    // 1. UPDATE ALBUM BY ID

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAlbum(int id, Album updatedAlbum)
    {
        // check if the provided id from the url matches the id in the request body
        if (id != updatedAlbum.Id)
        {
            return BadRequest("Album ID in the URL does not match the Id in the request body...");
        }
        // find the existing album in our list
        var existingAlbum = await _context.Albums.FirstOrDefaultAsync(a => a.Id == id);

        if (existingAlbum is null)
        {
            return NotFound("Album not found...");
        }

        // update the album with the new data
        // var albumIndex = _context.Albums.IndexOf(existingAlbum);
        // _albums[albumIndex] = updatedAlbum;
        existingAlbum.AlbumTitle = updatedAlbum.AlbumTitle;
        existingAlbum.Rank = updatedAlbum.Rank;
        existingAlbum.Artist = updatedAlbum.Artist;
        existingAlbum.ReleaseYear = updatedAlbum.ReleaseYear;
        existingAlbum.Genre = updatedAlbum.Genre;
        existingAlbum.AlbumTitle = updatedAlbum.AlbumTitle;
        existingAlbum.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // return a success response
        // NoContent returns a 204 status code, which is standard for a successful PUT
        return NoContent();
    }

    // DELETE ENDPOINT (DELETE)

    // 1. DELETE AN ALBUM BY ID

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAlbumById(int id)
    {
        // find album to remove by id
        var albumToRemove = await _context.Albums.FirstOrDefaultAsync(a => a.Id == id);

        if (albumToRemove is null)
        {
            return NotFound("Album not found...");
        }
        // mark the album for deletion
        _context.Albums.Remove(albumToRemove);
        await _context.SaveChangesAsync();

        // return a success response

        return NoContent();
    }

    // JAM MASTER CHALLENGES

    // 1. FILTER ALBUMS BY GENRE (QUERY STRING)
    [HttpGet("filter")]
    public async Task<ActionResult<List<Album>>> GetAlbumByGenre(string? genre)
    {
        var query = _context.Albums.AsQueryable();

        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(a => a.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase));
        }

        var results = await query.ToListAsync();

        if (results.Count == 0)
        {
            return NotFound("No albums found with matching genres.");
        }
        return Ok(results);
    }

    // 2. SEARCH ALBUMS BY ARTIST OR TITLE (QUERY STRING)
    [HttpGet("search")]
    public async Task<ActionResult<List<Album>>> SearchByArtistOrTitle(string? term)
    {
        var query = _context.Albums.AsQueryable();
        if (!string.IsNullOrEmpty(term))
        {
            query = query.Where(a =>
                a.Artist.Contains(term, StringComparison.OrdinalIgnoreCase)
                || a.AlbumTitle.Contains(term, StringComparison.OrdinalIgnoreCase)
            );
        }
        var results = await query.ToListAsync();

        if (results.Count == 0)
        {
            return NotFound("No albums found with those search parameters.");
        }
        return Ok(results);
    }
}
