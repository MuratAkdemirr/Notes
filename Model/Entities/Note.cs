using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notes.Data;

namespace Notes.Model.Entities;

public class Note
{
    public int Id { get; set; }

    public string Text { get; set; }

    public string UserId { get; set; }                 
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    public NoteStatus Status { get; set; } = NoteStatus.Active;
    public enum NoteStatus
    {
        Archived = 0,
        Active = 1,
    }
}