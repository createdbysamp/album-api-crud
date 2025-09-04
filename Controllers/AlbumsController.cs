using System.Collections.Generic;
using System.Text.Json;
using AlbumsApiCrud.Classes;
using Microsoft.AspNetCore.Mvc;

namespace AlbumsApiCrud.Controllers;

// general wholesale controller

[ApiController]
[Route("api/albums")]
public class AlbumsController : ControllerBase
{
    // created mutable in-memory list
    private static List<Album> _albums = Album.GetAlbums();

    // READ ENDPOINTS (GET)

    // 1. GET ALL ALBUMS
    [HttpGet]
    public ActionResult<List<Album>> GetAllAlbums()
    {
        if (_albums.Count == 0)
        {
            return NotFound("No albums found. Something is wrong."); // returns 404 NOT FOUND status code
        }
        return Ok(_albums); // 200 OK status code with the album list
    }

    // 2. GET A SINGLE ALBUM BY ID
    [HttpGet("{id}")]
    public ActionResult<Album> GetAlbumById(int id)
    {
        // use LINQ to find the album with the matching ID
        var searchedAlbum = _albums.FirstOrDefault(album => album.Id == id);

        if (searchedAlbum is null)
        {
            return NotFound("Specified album not found.."); // 404 not found
        }

        return Ok(searchedAlbum); // returns 200 OK with the album object
    }

    // CREATE ENDPOINT (POST)

    // 1. CREATE A NEW ALBUM

    [HttpPost]
    public ActionResult<Album> CreateAlbum([FromBody] Album newAlbum)
    {
        // auto generate new id
        var nextId = _albums.Count != 0 ? _albums.Max(a => a.Id) + 1 : 1;
        newAlbum.Id = nextId;

        // add new album to in-memory list
        _albums.Add(newAlbum);

        // return a success response
        // CreatedAtAction helper method returns a 201 Created status code
        return CreatedAtAction(nameof(GetAlbumById), new { id = newAlbum.Id }, newAlbum);
    }

    // UPDATE ENDPOINT (PUT)

    // 1. UPDATE ALBUM BY ID

    [HttpPut("{id}")]
    public ActionResult UpdateAlbum(int id, [FromBody] Album updatedAlbum)
    {
        // check if the provided id from the url matches the id in the request body
        if (id != updatedAlbum.Id)
        {
            return BadRequest("Album ID in the URL does not match the Id in the request body...");
        }
        // find the existing album in our list
        var existingAlbum = _albums.FirstOrDefault(a => a.Id == id);

        if (existingAlbum == null)
        {
            return NotFound("Album not found...");
        }

        // update the album with the new data
        var albumIndex = _albums.IndexOf(existingAlbum);
        _albums[albumIndex] = updatedAlbum;

        // return a success response
        // NoContent returns a 204 status code, which is standard for a successful PUT
        return NoContent();
    }

    // DELETE ENDPOINT (DELETE)

    // 1. DELETE AN ALBUM BY ID

    [HttpDelete("{id}")]
    public ActionResult<Album> DeleteAlbumById(int id)
    {
        // find album to remove by id
        var albumToRemove = _albums.FirstOrDefault(a => a.Id == id);

        if (albumToRemove is null)
        {
            return NotFound("Album not found...");
        }
        // remove the album from the list
        _albums.Remove(albumToRemove);

        // return a success response

        return NoContent();
    }

    // JAM MASTER CHALLENGES

    // 1. FILTER ALBUMS BY GENRE (QUERY STRING)
    [HttpGet("filter")]
    public ActionResult<List<Album>> GetAlbumByGenre(string? genre)
    {
        var query = _albums.AsQueryable();

        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(a => a.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase));
        }

        var results = query.ToList();

        if (results.Count == 0)
        {
            return NotFound("No albums found with matching genres.");
        }
        return Ok(results);
    }

    // 2. SEARCH ALBUMS BY ARTIST OR TITLE (QUERY STRING)
    [HttpGet("search")]
    public ActionResult<List<Album>> SearchByArtistOrTitle(string? term)
    {
        var query = _albums.AsQueryable();
        if (!string.IsNullOrEmpty(term))
        {
            query = query.Where(a =>
                a.Artist.Contains(term, StringComparison.OrdinalIgnoreCase)
                || a.AlbumTitle.Contains(term, StringComparison.OrdinalIgnoreCase)
            );
        }
        var results = query.ToList();

        if (results.Count == 0)
        {
            return NotFound("No albums found with those search parameters.");
        }
        return Ok(results);
    }
}
