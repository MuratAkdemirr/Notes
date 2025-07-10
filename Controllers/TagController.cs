using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes.Data;
using Notes.Model.Dto;
using Notes.Model.Dto.Tag;
using Notes.Model.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Notes.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[SwaggerTag("Tag CRUD API'ları")]
public class TagController(AppDbContext context, UserManager<IdentityUser> userManager) : ControllerBase
{
    // GET
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("")]
    [SwaggerOperation("Bütün etiketleri (Tag) getirir.")]
    public async Task<IActionResult> GetAll()
    {
        var tags = await context.Tags
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            })
            .ToListAsync();

        return Ok(tags);
    }


    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{name}")]
    [SwaggerOperation("Etiket ile bağlı olan notları getirir.")]
    public async Task<IActionResult> GetNotesByTag(string name)
    {
        var userId = userManager.GetUserId(User);

        var tag = await context.Tags
            .Include(t => t.Notes)
            .ThenInclude(n => n.Tags)
            .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

        if (tag == null)
            return NotFound();

        var notes = tag.Notes.Select(note => new NoteDto
        {
            Id = note.Id,
            Text = note.Text,
            Tags = note.Tags.Select(t => t.Name).ToList(),
            CreatedOn = note.CreatedOn,
            ModifiedOn = note.ModifiedOn
        });

        return Ok(notes);
    }

    // PUT 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation("İlgili etiketi günceller.")]
    [HttpPut("{name}")]
    public async Task<IActionResult> UpdateByName(string name, [FromBody] TagDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest();
        }

        var tag = await context.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

        if (tag == null)
        {
            return NotFound();
        }

        var existing = await context.Tags
            .AnyAsync(t => t.Name.ToLower() == dto.Name.ToLower() && t.Id != tag.Id);

        if (existing)
        {
            return Conflict();
        }

        tag.Name = dto.Name.Trim().ToLower();

        await context.SaveChangesAsync();

        return NoContent();
    }
    
    // DELETE
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{string}")]
    public async Task<IActionResult> Delete(string name)
    {
        var tag = await context.Tags
            .Include(t => t.Notes)
            .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

        if (tag == null)
            return NotFound();

        context.Tags.Remove(tag);
        await context.SaveChangesAsync();

        return NoContent();
    }
}