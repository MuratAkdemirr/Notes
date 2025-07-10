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
[SwaggerTag("Tag CRUD API'ları")]
public class ArchiveController(AppDbContext context, UserManager<IdentityUser> userManager) : ControllerBase
{
    //PUT

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Kullanıcının kendi Notunu arşivler.")]
    public async Task<IActionResult> Archive(int id)
    {
        var userId = userManager.GetUserId(User);

        var note = await context.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (note == null)
        {
            return NotFound();
        }

        note.Status = Note.NoteStatus.Archived;
        note.ModifiedOn = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id}/unarchive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Kullanıcının arşivlediği notu arşivden çıkartır.")]
    public async Task<IActionResult> UnArchive(int id)
    {
        var userId = userManager.GetUserId(User);

        var note = await context.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (note == null)
        {
            return NotFound();
        }

        if (note.Status != Note.NoteStatus.Archived)
        {
            return BadRequest();
        }

        note.Status = Note.NoteStatus.Active;
        note.ModifiedOn = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Ok();
    }

    //GET

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Kullanıcının arşivlenmiş notlarını getirir.")]
    public async Task<IActionResult> GetArchived()
    {
        var userId = userManager.GetUserId(User);

        var notes = await context.Notes
            .Where(n => n.UserId == userId && n.Status == Note.NoteStatus.Archived)
            .Include(n => n.Tags)
            .OrderByDescending(n => n.ModifiedOn)
            .Select(n => new NoteDto
            {
                Id = n.Id,
                Text = n.Text,
                Tags = n.Tags.Select(t => t.Name).ToList(),
                CreatedOn = n.CreatedOn,
                ModifiedOn = n.ModifiedOn
            })
            .ToListAsync();

        return Ok(notes);
    }
}