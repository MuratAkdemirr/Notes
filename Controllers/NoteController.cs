using FluentEmail.Core;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes.Data;
using Notes.Model.Dto;
using Notes.Model.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Notes.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[SwaggerTag("Note CRUD API'ları.")]
public class NoteController(AppDbContext context, UserManager<IdentityUser> userManager) : ControllerBase
{
    // GET
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation("Kullanıcının kendine ait Not'ları getirir. (Tarihe göre en yakın Tarih her zaman üstte olur.) ")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = userManager.GetUserId(User);

        var note = await context.Notes
            .Include(n => n.Tags)
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (note == null)
            return NotFound();

        var dto = new NoteDto
        {
            Id = note.Id,
            Title = note.Title,
            Text = note.Text,
            Tags = note.Tags.Select(t => t.Name).ToList(),
            CreatedOn = note.CreatedOn,
            ModifiedOn = note.ModifiedOn
        };

        return Ok(dto);
    }
    
    // POST
    
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation("Kullanıcı kendine ait bir Not oluşturur.")]
    [HttpPost("")]
    public async Task<IActionResult> Create(CreateNoteDto dto)
    {
        var userId = userManager.GetUserId(User);

        var note = new Note
        {
            Title = dto.Title,
            Text = dto.Text,
            UserId = userId,
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow,
            Tags = new List<Tag>()
        };

        var tagNames = dto.Tags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToLower())
            .Distinct()
            .ToList();

        foreach (var tagName in tagNames)
        {
            var tag = await context.Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == tagName);

            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                context.Tags.Add(tag);
            }

            note.Tags.Add(tag);
        }

        context.Notes.Add(note);
        await context.SaveChangesAsync();

        var result = new NoteDto
        {
            Id = note.Id,
            Title = note.Title,
            Text = note.Text,
            Tags = note.Tags.Select(t => t.Name).ToList(),
            CreatedOn = note.CreatedOn,
            ModifiedOn = note.ModifiedOn
        };

        return CreatedAtAction(nameof(GetById), new { id = note.Id }, result);
    }
    
    // PUT
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation("Kullanıcı kendine ait Notu günceller.")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateNoteDto dto)
    {
        var userId = userManager.GetUserId(User);

        var note = await context.Notes
            .Include(n => n.Tags)
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (note == null)
            return NotFound();
        note.Title = note.Title;
        note.Text = dto.Text;
        note.ModifiedOn = DateTime.UtcNow;
        note.Tags.Clear();

        var tagNames = dto.Tags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToLower())
            .Distinct()
            .ToList();

        foreach (var tagName in tagNames)
        {
            var tag = await context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == tagName);
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                context.Tags.Add(tag);
            }

            note.Tags.Add(tag);
        }

        await context.SaveChangesAsync();
        return Ok();
    }
    
    
    //DELETE
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Kullanıcı kendine ait Notu siler.")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = userManager.GetUserId(User);

        var note = await context.Notes
            .Include(n => n.Tags)
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (note == null)
            return NotFound();

        context.Notes.Remove(note);
        await context.SaveChangesAsync();

        return NoContent();
    }
}